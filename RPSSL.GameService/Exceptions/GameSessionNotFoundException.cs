namespace RPSSL.GameService.Exceptions;

public class GameSessionNotFoundException : Exception
{
  public GameSessionNotFoundException(string gameCode)
      : base($"Game session with code '{gameCode}' was not found.")
  {
  }
}
