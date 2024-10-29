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

  [Theory]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Rock, GameResultEnum.Tie)]
  [InlineData(ChoiceEnum.Paper, ChoiceEnum.Paper, GameResultEnum.Tie)]
  [InlineData(ChoiceEnum.Scissors, ChoiceEnum.Scissors, GameResultEnum.Tie)]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Scissors, GameResultEnum.Win)]
  [InlineData(ChoiceEnum.Scissors, ChoiceEnum.Paper, GameResultEnum.Win)]
  [InlineData(ChoiceEnum.Paper, ChoiceEnum.Rock, GameResultEnum.Win)]
  [InlineData(ChoiceEnum.Rock, ChoiceEnum.Paper, GameResultEnum.Lose)]
  [InlineData(ChoiceEnum.Scissors, ChoiceEnum.Rock, GameResultEnum.Lose)]
  [InlineData(ChoiceEnum.Paper, ChoiceEnum.Scissors, GameResultEnum.Lose)]
  public void GetResult_ShouldReturnExpectedResult(ChoiceEnum playerOneChoice, ChoiceEnum playerTwoChoice, GameResultEnum expectedResult)
  {

    var result = _strategy.GetResult(playerOneChoice, playerTwoChoice);
    Assert.Equal(expectedResult, result);
  }
}