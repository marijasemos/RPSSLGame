using RPSSL.GameService.Models;

namespace RPSSL.GameService.Interfaces;

/// <summary>
/// Interface for managing game sessions.
/// </summary>
public interface IGameSessionService
{
  /// <summary>
  /// Creates a new game session.
  /// </summary>
  Task<GameInfo> CreateSessionAsync();

  /// <summary>
  /// Allows a player to join an existing session.
  /// </summary>
  Task<string> JoinSessionAsync(string gameCode);

  /// <summary>
  /// Makes a move in the game for a specific player.
  /// </summary>
  Task<GameSession> MakeMoveAsync(string gameCode, string playerId, int choice);

  /// <summary>
  /// Resets the data for an existing game session.
  /// </summary>
  Task ResetDataForSessionAsync(GameSession session);


  /// <summary>
  /// Get the data for an existing game session.
  /// </summary>
  Task<GameSession> GetSessionAsync(string gameCode);

  /// <summary>
  /// Remove existing game session.
  /// </summary>
  Task RemoveSessionAsync(string gameCode);
}

