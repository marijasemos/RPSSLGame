using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Strategies;

namespace RPSSL.GameService.Services;

/// <summary>
/// Factory for creating game strategies based on context.
/// </summary>
public class GameStrategyFactory : IGameStrategyFactory
{
  public IGameStrategy CreateSinglePlayerStrategy()
  {
    return new RPSSLGameStrategy();
  }

  public IGameStrategy CreateTwoPlayerStrategy(string playerOneId, string playerTwoId)
  {
    return new TwoPlayerStrategy(playerOneId, playerTwoId);
  }
}
