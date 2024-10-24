using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPSSL.GameService.Interfaces;
using RPSSL.GameService.Models;
using RPSSL.GameService.Models.Enums;

namespace RPSSL.GameService.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PlayController : ControllerBase
{
  private readonly IGameStrategyFactory _strategyFactory;
  private readonly ILogger<PlayController> _logger;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly IGameSessionService _gameService;

  public PlayController(IGameStrategyFactory strategyFactory, IGameSessionService gameService, ILogger<PlayController> logger, IHttpClientFactory httpClientFactory)
  {
    _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _httpClientFactory = httpClientFactory;
    _gameService = gameService;

  }
  [HttpPost]
  public async Task<IActionResult> PlayRound([FromBody] PlayRequest request)
  {
    try
    {
      // Use IHttpClientFactory to create a named client
      var client = _httpClientFactory.CreateClient("ChoiceServiceClient");

      var response = await client.GetAsync("/api/choices/choice");

      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Failed to get computer choice. StatusCode: {StatusCode}", response.StatusCode);
        return StatusCode((int)response.StatusCode, "Failed to get computer choice.");
      }

      var responseBody = await response.Content.ReadAsStringAsync();
      var computerChoice = JsonConvert.DeserializeObject<Choice>(responseBody);

      var strategy = _strategyFactory.CreateSinglePlayerStrategy();
      var result = strategy.GetResult((ChoiceEnum)request.PlayerChoice, (ChoiceEnum)computerChoice.Id);

      return Ok(new PlayResponse
      {
        Results = result,
        Player = request.PlayerChoice,
        Computer = computerChoice.Id
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error playing game.");
      return StatusCode(500, $"Error playing game: {ex.Message}");
    }
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateGame()
  {
    try
    {
      _logger.LogInformation("Attempting to create a new game session.");

      // Call the service to create a new game session
      var gameInfo = await _gameService.CreateSessionAsync();

      if (gameInfo == null)
      {
        _logger.LogWarning("Game session creation returned null.");
        return StatusCode(500, "Failed to create a game session. Please try again.");
      }

      _logger.LogInformation("Game session successfully created with GameCode: {GameCode}", gameInfo.GameCode);
      return Ok(gameInfo);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while creating a game session.");
      return StatusCode(500, "An unexpected error occurred. Please try again later.");
    }
  }
}


