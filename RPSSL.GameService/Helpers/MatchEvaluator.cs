using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Helpers;

/// <summary>
/// Provides helper methods to evaluate game moves and determine if a move is a winning one.
/// </summary>
public static class MatchEvaluator
{
  // Define winning combinations using a dictionary for more intuitive rules configuration.
  private static readonly Dictionary<ChoiceEnum, List<ChoiceEnum>> WinningMoves = new()
        {
            { ChoiceEnum.Rock, new List<ChoiceEnum> { ChoiceEnum.Scissors, ChoiceEnum.Lizard } },
            { ChoiceEnum.Paper, new List<ChoiceEnum> { ChoiceEnum.Rock, ChoiceEnum.Spock } },
            { ChoiceEnum.Scissors, new List<ChoiceEnum> { ChoiceEnum.Paper, ChoiceEnum.Lizard } },
            { ChoiceEnum.Lizard, new List<ChoiceEnum> { ChoiceEnum.Spock, ChoiceEnum.Paper } },
            { ChoiceEnum.Spock, new List<ChoiceEnum> { ChoiceEnum.Rock, ChoiceEnum.Scissors } }
        };

  /// <summary>
  /// Determines if the player's choice beats the opponent's choice.
  /// </summary>
  public static bool IsWinningMove(ChoiceEnum player, ChoiceEnum opponent)
  {
    return WinningMoves.TryGetValue(player, out var beats) && beats.Contains(opponent);
  }
}
