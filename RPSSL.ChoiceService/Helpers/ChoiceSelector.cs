using RPSSL.ChoiceService.Models;
using RPSSL.ChoiceService.Models.Enums;

namespace RPSSL.ChoiceService.Helpers;

public static class ChoiceSelector
{
  /// <summary>
  /// Determines a choice based on a given random number.
  /// </summary>
  /// <param name="randomNumber">The random number used to select a choice.</param>
  /// <returns>A <see cref="Choice"/> object representing the selected choice.</returns>
  public static Choice GetChoiceFromRandomNumber(int randomNumber)
  {

    // Determine the choice index based on the random number.
    var enumValues = Enum.GetValues<ChoiceEnum>();
    int enumIndex = (randomNumber - 1) % enumValues.Length;

    var selectedEnum = enumValues[enumIndex];

    return new Choice
    {
      Id = (int)selectedEnum,
      Name = selectedEnum.ToString()
    };
  }
}

