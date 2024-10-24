using FluentAssertions;
using RPSSL.GameService.Models.Enums;
using RPSSL.GameService.Strategies;

namespace RPSSL.GameService.Tests.Strategies;
public class TwoPlayerStrategyTests
{
  private readonly TwoPlayerStrategy _strategy;
  private readonly string _playerOneId = "Player1";
  private readonly string _playerTwoId = "Player2";

  public TwoPlayerStrategyTests()
  {
    _strategy = new TwoPlayerStrategy(_playerOneId, _playerTwoId);
  }

  [Fact]
  public void GetResult_ShouldReturnCorrectPlayerId()
  {
    // Act
    var result = _strategy.GetResult(ChoiceEnum.Rock, ChoiceEnum.Scissors);

    // Assert
    result.Should().Be(_playerOneId);

    result = _strategy.GetResult(ChoiceEnum.Paper, ChoiceEnum.Rock);
    result.Should().Be(_playerOneId);

    result = _strategy.GetResult(ChoiceEnum.Lizard, ChoiceEnum.Rock);
    result.Should().Be(_playerTwoId);
  }
}