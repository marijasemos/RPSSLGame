using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Models.Enums;
using static RPSSL.GameService.Helpers.MatchEvaluator;

namespace RPSSL.GameService.Strategies;

/// <summary>
/// Game strategy implementation for RPSSL against computer.
/// </summary>
public class RPSSLGameStrategy : IGameStrategy
{
  /// <inheritdoc />
  public GameResultEnum GetResult(ChoiceEnum playerOneChoice, ChoiceEnum playerTwoChoice)
  {
    if (playerOneChoice == playerTwoChoice)
    {
      return GameResultEnum.Tie;
    }

    return IsWinningMove(playerOneChoice, playerTwoChoice) ? GameResultEnum.Win : GameResultEnum.Lose;
  }
}
