using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using RPSSL.GameService.Exceptions;
using RPSSL.GameService.Models;
using RPSSL.GameService.Models.Enums;
using RPSSL.GameService.Services;
using StackExchange.Redis;
using System.Text;

namespace RPSSL.GameService.Tests.Services;
public class RedisGameSessionServiceTests
{
  private readonly Mock<IDistributedCache> _mockCache;
  private readonly Mock<IConnectionMultiplexer> _mockMultiplexer;
  private readonly Mock<ILogger<RedisGameSessionService>> _mockLogger;
  private readonly RedisGameSessionService _service;

  public RedisGameSessionServiceTests()
  {
    _mockCache = new Mock<IDistributedCache>();
    _mockLogger = new Mock<ILogger<RedisGameSessionService>>();
    _mockMultiplexer = new Mock<IConnectionMultiplexer>();
    _service = new RedisGameSessionService(_mockMultiplexer.Object, _mockCache.Object, _mockLogger.Object);
  }

  [Fact]
  public async Task CreateSessionAsync_ShouldReturnGameInfo()
  {
    // Arrange
    var fakeGameCode = "ABC123";
    var fakePlayerId = "PLAYER1";

    _mockCache.Setup(c => c.SetAsync(
        It.IsAny<string>(),
        It.IsAny<byte[]>(),
        It.IsAny<DistributedCacheEntryOptions>(),
        It.IsAny<CancellationToken>()
    )).Verifiable();

    // Act
    var result = await _service.CreateSessionAsync();

    // Assert
    result.Should().NotBeNull();
    result.GameCode.Should().HaveLength(8);
    result.PlayerId.Should().HaveLength(15);
    _mockCache.Verify();
  }

  [Fact]
  public async Task JoinSessionAsync_ShouldReturnPlayerTwoId()
  {
    // Arrange
    var gameCode = "GAME123";
    var session = new GameSession
    {
      GameCode = gameCode,
      Status = Models.Enums.GameStatusEnum.Waiting,
      PlayerOneId = "PlayerOne123"
    };

    // Serialize the session to match how it would be stored in Redis
    var sessionData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(session));

    _mockCache.Setup(c => c.GetAsync(gameCode, It.IsAny<CancellationToken>()))
              .ReturnsAsync(sessionData);

    // Act
    var result = await _service.JoinSessionAsync(gameCode);

    // Assert
    result.Should().NotBeNullOrEmpty();
    _mockCache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task MakeMoveAsync_ShouldThrowException_WhenGameSessionNotFound()
  {
    // Arrange
    string gameCode = "testGame";
    string playerId = "player1";
    int choice = 1;

    _mockCache.Setup(x => x.GetAsync(gameCode, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[])null);
    //_mockCache.Setup(x => x.GetStringAsync(gameCode)).ReturnsAsync((string)null);

    // Act & Assert
    await Assert.ThrowsAsync<GameSessionNotFoundException>(
        () => _service.MakeMoveAsync(gameCode, playerId, choice));
  }

  [Fact]
  public async Task MakeMoveAsync_ShouldThrowException_WhenGameIsNotInProgress()
  {
    // Arrange
    string gameCode = "GAME123";
    string playerId = "player1";
    int choice = 1;

    var session = new GameSession
    {
      GameCode = gameCode,
      Status = GameStatusEnum.Finished // Not in-progress
    };
    var sessionData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(session));

    _mockCache.Setup(c => c.GetAsync(gameCode, It.IsAny<CancellationToken>())).ReturnsAsync(sessionData);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidGameStatusException>(
        () => _service.MakeMoveAsync(gameCode, playerId, choice));
  }

  [Fact]
  public async Task MakeMoveAsync_ShouldSetPlayerChoice_WhenValidMoveIsMade()
  {
    // Arrange
    string gameCode = "GAME123";
    string playerId = "player1";
    int choice = 1;

    var session = new GameSession
    {
      GameCode = gameCode,
      Status = GameStatusEnum.InProgress,
      PlayerOneId = playerId
    };

    var sessionData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(session));

    _mockCache.Setup(c => c.GetAsync(gameCode, It.IsAny<CancellationToken>())).ReturnsAsync(sessionData);


    // Act
    var updatedSession = await _service.MakeMoveAsync(gameCode, playerId, choice);

    // Assert
    Assert.Equal(choice, updatedSession.PlayerOneChoice);
    Assert.Null(updatedSession.PlayerTwoChoice);
  }

  [Fact]
  public async Task MakeMoveAsync_ShouldConcludeGame_WhenBothPlayersHaveMadeChoices()
  {
    // Arrange
    string gameCode = "GAME123";
    string playerOneId = "player1";
    string playerTwoId = "player2";
    int playerOneChoice = 1;
    int playerTwoChoice = 2;

    var session = new GameSession
    {
      GameCode = gameCode,
      Status = GameStatusEnum.InProgress,
      PlayerOneId = playerOneId,
      PlayerTwoId = playerTwoId,
      PlayerOneChoice = playerOneChoice
    };

    var sessionData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(session));

    _mockCache.Setup(c => c.GetAsync(gameCode, It.IsAny<CancellationToken>())).ReturnsAsync(sessionData);



    // Act
    var updatedSession = await _service.MakeMoveAsync(gameCode, playerTwoId, playerTwoChoice);

    // Assert
    Assert.Equal(GameStatusEnum.Finished, updatedSession.Status);
    Assert.Equal(playerOneChoice, updatedSession.PlayerOneChoice);
    Assert.Equal(playerTwoChoice, updatedSession.PlayerTwoChoice);
  }

}


