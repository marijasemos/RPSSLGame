using FluentAssertions;
using RPSSL.GameService.Helpers;
using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Tests.Helpers;
public class MatchEvaluatorTests
{
  [Theory]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Scissors, true)]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Lizard, true)]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Paper, false)]
  [InlineData(ChoiceEnum.Spock, ChoiceEnum.Rock, true)]
  [InlineData(ChoiceEnum.Lizard, ChoiceEnum.Scissors, false)]
  public void IsWinningMove_ShouldReturnExpectedResult(ChoiceEnum player, ChoiceEnum opponent, bool expectedResult)
  {
    // Act
    var result = MatchEvaluator.IsWinningMove(player, opponent);

    // Assert
    result.Should().Be(expectedResult);
  }
}
