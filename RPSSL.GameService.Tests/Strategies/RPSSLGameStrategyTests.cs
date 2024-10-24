using FluentAssertions;
using RPSSL.GameService.Models.Enums;
using RPSSL.GameService.Strategies;

namespace RPSSL.GameService.Tests.Strategies;
public class RPSSLGameStrategyTests
{
  private readonly RPSSLGameStrategy _strategy;

  public RPSSLGameStrategyTests()
  {
    _strategy = new RPSSLGameStrategy();
  }

  [Theory]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Scissors, "win")]
  [InlineData(ChoiceEnum.Paper, ChoiceEnum.Paper, "tie")]
  [InlineData(ChoiceEnum.Lizard, ChoiceEnum.Scissors, "lose")]
  public void GetResult_ShouldReturnExpectedOutcome(ChoiceEnum playerOneChoice, ChoiceEnum playerTwoChoice, string expectedOutcome)
  {
    // Act
    var result = _strategy.GetResult(playerOneChoice, playerTwoChoice);

    // Assert
    result.Should().Be(expectedOutcome);
  }
}
