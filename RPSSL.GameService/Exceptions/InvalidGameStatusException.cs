using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Exceptions;

public class InvalidGameStatusException : Exception
{
  public InvalidGameStatusException(string gameCode, GameStatusEnum status)
      : base($"Invalid game status '{status}' for game code '{gameCode}'.")
  {
  }
}
