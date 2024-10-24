using FluentAssertions;
using RPSSL.ChoiceService.Helpers;
using RPSSL.ChoiceService.Models.Enums;

namespace RPSSL.ChoiceService.Tests.Helpers;
public class ChoiceSelectorTests
{
  [Theory]
  [InlineData(1, ChoiceEnum.Rock)]
  [InlineData(2, ChoiceEnum.Paper)]
  [InlineData(3, ChoiceEnum.Scissors)]
  [InlineData(4, ChoiceEnum.Lizard)]
  [InlineData(5, ChoiceEnum.Spock)]
  public void GetChoiceFromRandomNumber_ShouldReturnCorrectChoice_WhenRandomNumberIsWithinRange(int randomNumber, ChoiceEnum expectedChoice)
  {
    // Act
    var result = ChoiceSelector.GetChoiceFromRandomNumber(randomNumber);

    // Assert
    result.Id.Should().Be((int)expectedChoice);
    result.Name.Should().Be(expectedChoice.ToString());
  }

  [Theory]
  [InlineData(6, ChoiceEnum.Rock)]  // 6 % 5 == 1
  [InlineData(7, ChoiceEnum.Paper)] // 7 % 5 == 2
  [InlineData(10, ChoiceEnum.Spock)] // 10 % 5 == 0
  [InlineData(11, ChoiceEnum.Rock)] // 11 % 5 == 1
  public void GetChoiceFromRandomNumber_ShouldReturnCorrectChoice_WhenRandomNumberIsOutsideRange(int randomNumber, ChoiceEnum expectedChoice)
  {
    // Act
    var result = ChoiceSelector.GetChoiceFromRandomNumber(randomNumber);

    // Assert
    result.Id.Should().Be((int)expectedChoice);
    result.Name.Should().Be(expectedChoice.ToString());
  }

}
