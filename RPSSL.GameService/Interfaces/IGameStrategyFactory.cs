namespace RPSSL.GameService.Interfaces;

/// <summary>
/// Factory interface for creating game strategies.
/// </summary>
public interface IGameStrategyFactory
{
  /// <summary>
  /// Creates a single-player game strategy.
  /// </summary>
  IGameStrategy CreateSinglePlayerStrategy();

  /// <summary>
  /// Creates a two-player game strategy.
  /// </summary>
  /// <param name="playerOneId">The ID of player one.</param>
  /// <param name="playerTwoId">The ID of player two.</param>
  IGameStrategy CreateTwoPlayerStrategy(string playerOneId, string playerTwoId);
}
