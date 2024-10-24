using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Interfaces;

/// <summary>
/// Defines the contract for determining the result of a game round.
/// </summary>
public interface IGameStrategy
{
  /// <summary>
  /// Determines the result of a game round between two choices.
  /// </summary>
  /// <param name="playerOneChoice">The choice made by player one.</param>
  /// <param name="playerTwoChoice">The choice made by player two.</param>
  /// <returns>The result of the round: "win", "lose", or "tie".</returns>
  string GetResult(ChoiceEnum playerOneChoice, ChoiceEnum playerTwoChoice);
}
