using Microsoft.AspNetCore.SignalR;
using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Models.Enums;
using static RPSSL.GameService.Helpers.MatchEvaluator;
namespace RPSSL.GameService.Hubs;

/// <summary>
/// SignalR Hub to manage real-time game interactions.
/// </summary>
public class GameHub : Hub
{
  private readonly IGameSessionService _gameService;

  public GameHub(IGameSessionService gameService)
  {
    _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
  }

  public override async Task OnConnectedAsync()
  {
    var gameCode = Context.GetHttpContext()?.Request.Query["gameCode"];
    var sessiom = await _gameService.GetSessionAsync(gameCode);
    if (!string.IsNullOrEmpty(gameCode) && sessiom != null)
    {
      await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
    }
    await base.OnConnectedAsync();
  }

  /// <summary>
  /// Allows a player to join an existing game session.
  /// </summary>
  public async Task JoinGame(string gameCode)
  {
    if (string.IsNullOrEmpty(gameCode))
    {
      await Clients.Caller.SendAsync("Error", "Invalid game code.");
      return;
    }

    try
    {
      var playerId = await _gameService.JoinSessionAsync(gameCode);
      if (!string.IsNullOrEmpty(playerId))
      {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);
        await Clients.Group(gameCode).SendAsync("Connected", true);
        await Clients.Caller.SendAsync("PlayerId", playerId);
      }
      else
      {
        await Clients.Caller.SendAsync("Error", "Unable to join game. Game session might be full or not in a waiting state.");
      }
    }
    catch (Exception ex)
    {
      await Clients.Caller.SendAsync("Error", $"An error occurred while joining the game: {ex.Message}");
    }
  }

  /// <summary>
  /// Handles sending a player's move to the game and processes the game state.
  /// </summary>
  public async Task SendMove(string gameCode, string playerId, int choice)
  {
    if (string.IsNullOrEmpty(gameCode) || string.IsNullOrEmpty(playerId))
    {
      await Clients.Caller.SendAsync("Error", "Invalid game or player identifier.");
      return;
    }

    try
    {
      var result = await _gameService.MakeMoveAsync(gameCode, playerId, choice);
      if (result.Status == Models.Enums.GameStatusEnum.Finished)
      {

        GameResultEnum statusCaller;

        if (result.PlayerOneChoice == result.PlayerTwoChoice)
          statusCaller = GameResultEnum.Tie;
        else if (IsWinningMove((ChoiceEnum)result.PlayerOneChoice, (ChoiceEnum)result.PlayerTwoChoice) && result.PlayerOneId == playerId)
          statusCaller = GameResultEnum.Win;
        else if (IsWinningMove((ChoiceEnum)result.PlayerTwoChoice, (ChoiceEnum)result.PlayerOneChoice) && result.PlayerTwoId == playerId)
          statusCaller = GameResultEnum.Win;
        else
          statusCaller = GameResultEnum.Lose;


        GameResultEnum statusOponent = statusCaller switch
        {
          GameResultEnum.Win => GameResultEnum.Lose,
          GameResultEnum.Lose => GameResultEnum.Win,
          _ => GameResultEnum.Tie
        };
        var statusCallerText = statusCaller.ToString();
        var statusOponentText = statusOponent.ToString();
        var opponentChoice = result.PlayerOneId == playerId ? result.PlayerTwoChoice : result.PlayerOneChoice;

        await Clients.Caller.SendAsync("Result", new { choice, opponentChoice, status = statusCallerText });

        await Clients.OthersInGroup(gameCode).SendAsync("Result", new { choice = opponentChoice, opponentChoice = choice, status = statusOponentText });


        // Reset game data for the next round
        await _gameService.ResetDataForSessionAsync(result);
      }
      else
        await Clients.Caller.SendAsync("Waiting", "Waiting for the opponent to make a move.");

    }
    catch (Exception ex)
    {
      await Clients.Caller.SendAsync("Error", $"An error occurred while processing the move: {ex.Message}");
    }
  }

  /// <summary>
  /// Notifies the end of the game to all players in the session.
  /// </summary>
  public async Task GameOver(string gameCode, bool? isTimeExpired)
  {
    if (string.IsNullOrEmpty(gameCode))
    {
      await Clients.Caller.SendAsync("Error", "Invalid game code.");
      return;
    }
    if (isTimeExpired == true)
    {
      await Clients.Group(gameCode).SendAsync("GameOver", "The time is up, the game is over.");

    }
    else
    {
      await Clients.OthersInGroup(gameCode).SendAsync("GameOver", "The game is over, the other player has left the game!");
      await Clients.Caller.SendAsync("GameOver", "The game is over. You have stopped the game!");
    }
    await _gameService.RemoveSessionAsync(gameCode);

  }
}

