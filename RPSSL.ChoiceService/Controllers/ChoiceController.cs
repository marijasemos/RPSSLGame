using Microsoft.AspNetCore.Mvc;
using RPSSL.ChoiceService.Helpers;
using RPSSL.ChoiceService.Interfaces;
using RPSSL.ChoiceService.Models.Enums;
using System.Net.Mime;

namespace RPSSL.ChoiceService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Produces(MediaTypeNames.Application.Json)]
  public class ChoicesController : ControllerBase
  {
    private readonly IRandomNumberService _randomNumberService;
    private readonly ILogger<ChoicesController> _logger;

    public ChoicesController(IRandomNumberService randomNumberService, ILogger<ChoicesController> logger)
    {
      _randomNumberService = randomNumberService ?? throw new ArgumentNullException(nameof(randomNumberService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all available choices.
    /// </summary>
    /// <returns>A list of choices with their IDs and names.</returns>
    [HttpGet]
    [Route("")]
    public IActionResult GetChoices()
    {
      var choices = Enum.GetValues<ChoiceEnum>()
          .Select(choice => new { Id = (int)choice, Name = choice.ToString() })
          .ToList();

      return Ok(choices);
    }

    /// <summary>
    /// Generates a random choice based on an external random number service.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>A randomly selected choice.</returns>
    [HttpGet("choice")]
    public async Task<IActionResult> GetRandomChoice(CancellationToken cancellationToken)
    {
      try
      {
        // Fetch a random number from the external service
        int randomNumber = await _randomNumberService.GetRandomNumberAsync(cancellationToken);

        // Determine choice based on the random number
        var choice = ChoiceSelector.GetChoiceFromRandomNumber(randomNumber);

        return Ok(choice);
      }
      catch (OperationCanceledException)
      {
        _logger.LogWarning("Random choice generation was canceled.");
        return StatusCode(499, "Request was canceled by the client.");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error generating random choice.");
        return StatusCode(500, "An error occurred while fetching a random choice. Please try again later.");
      }
    }
  }
}
