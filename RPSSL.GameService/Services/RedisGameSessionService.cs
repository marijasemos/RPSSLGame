using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using RPSSL.GameService.Exceptions;
using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Models;
using RPSSL.GameService.Models.Enums;
using StackExchange.Redis;

namespace RPSSL.GameService.Services;

/// <summary>
/// Service for managing game sessions using Redis as the storage backend.
/// </summary>
public class RedisGameSessionService : IGameSessionService
{
  private readonly IDistributedCache _distributedCache;
  private readonly IDatabase _redisDb;

  private readonly ILogger<RedisGameSessionService> _logger;

  public RedisGameSessionService(IConnectionMultiplexer redis, IDistributedCache distributedCache, ILogger<RedisGameSessionService> logger)
  {
    _redisDb = redis.GetDatabase();

    _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc />
  public async Task<GameInfo> CreateSessionAsync()
  {
    var gameCode = GenerateUniqueId(8);
    var playerId = GenerateUniqueId(15);

    var session = new GameSession
    {
      GameCode = gameCode,
      Status = GameStatusEnum.Waiting,
      PlayerOneId = playerId
    };
    var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(2))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));


    await _distributedCache.SetStringAsync(gameCode, JsonConvert.SerializeObject(session), options);

    _logger.LogInformation("Created new game session. GameCode: {GameCode}, PlayerOneId: {PlayerOneId}", gameCode, playerId);

    return new GameInfo { GameCode = gameCode, PlayerId = playerId };
  }

  /// <inheritdoc />
  public async Task<string> JoinSessionAsync(string gameCode)
  {
    var session = await GetSessionAsync(gameCode);
    if (session == null || session.Status != GameStatusEnum.Waiting)
    {
      _logger.LogWarning("Failed to join session. GameCode: {GameCode} is not available or not in a waiting state.", gameCode);
      return string.Empty;
    }

    session.PlayerTwoId = GenerateUniqueId(15);
    session.Status = GameStatusEnum.InProgress;
    await SaveSessionAsync(gameCode, session);

    _logger.LogInformation("Player joined session. GameCode: {GameCode}, PlayerTwoId: {PlayerTwoId}", gameCode, session.PlayerTwoId);
    return session.PlayerTwoId;
  }

  /// <inheritdoc />
  public async Task<GameSession> MakeMoveAsync(string gameCode, string playerId, int choice)
  {
    var session = await GetSessionAsync(gameCode);
    if (session == null)
    {
      _logger.LogWarning("Game session not found. GameCode: {GameCode}", gameCode);
      throw new GameSessionNotFoundException(gameCode);
    }

    if (session.Status != GameStatusEnum.InProgress)
    {
      _logger.LogWarning("Game is not in-progress. GameCode: {GameCode}", gameCode);
      throw new InvalidGameStatusException(gameCode, session.Status);
    }

    // Process the player's move
    if (session.PlayerOneId == playerId)
      session.PlayerOneChoice = choice;
    else if (session.PlayerTwoId == playerId)
      session.PlayerTwoChoice = choice;

    // If both players have made their choices, conclude the game
    if (session.PlayerOneChoice.HasValue && session.PlayerTwoChoice.HasValue)
    {
      session.Status = GameStatusEnum.Finished;
      // Add game logic to determine the winner here
      _logger.LogInformation("Game session completed. GameCode: {GameCode}", gameCode);
    }

    await SaveSessionAsync(gameCode, session);
    return session;
  }

  /// <inheritdoc />
  public async Task ResetDataForSessionAsync(GameSession session)
  {
    if (session == null) throw new ArgumentNullException(nameof(session));

    session.PlayerOneChoice = null;
    session.PlayerTwoChoice = null;
    session.Status = GameStatusEnum.InProgress;

    await SaveSessionAsync(session.GameCode, session);
    _logger.LogInformation("Game session reset. GameCode: {GameCode}", session.GameCode);
  }

  private string GenerateUniqueId(int length)
  {
    return Guid.NewGuid().ToString("N").Substring(0, length);
  }

  private async Task SaveSessionAsync(string gameCode, GameSession session)
  {
    try
    {
      var serializedSession = JsonConvert.SerializeObject(session);
      await _distributedCache.SetStringAsync(gameCode, serializedSession);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to save session to Redis. GameCode: {GameCode}", gameCode);
      throw;
    }
  }

  public async Task<GameSession> GetSessionAsync(string gameCode)
  {
    try
    {
      var sessionData = await _distributedCache.GetStringAsync(gameCode);
      if (string.IsNullOrEmpty(sessionData))
      {
        _logger.LogWarning("No data found for GameCode: {GameCode}", gameCode);
        return null;
      }

      return JsonConvert.DeserializeObject<GameSession>(sessionData);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to retrieve session from Redis. GameCode: {GameCode}", gameCode);
      return null;
    }
  }

  /// <inheritdoc />
  public async Task RemoveSessionAsync(string gameCode)
  {
    if (string.IsNullOrWhiteSpace(gameCode))
    {
      _logger.LogWarning("Attempted to remove a session with an empty or null GameCode.");
      throw new ArgumentException("GameCode cannot be null or empty.", nameof(gameCode));
    }

    try
    {
      await _distributedCache.RemoveAsync(gameCode);
      _logger.LogInformation("Removed game session. GameCode: {GameCode}", gameCode);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to remove session from Redis. GameCode: {GameCode}", gameCode);
      throw;
    }
  }
}

