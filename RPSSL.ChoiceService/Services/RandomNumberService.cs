using Newtonsoft.Json;
using RPSSL.ChoiceService.Exceptions;
using RPSSL.ChoiceService.Interfaces;

namespace RPSSL.ChoiceService.Services
{
  public class RandomNumberService : IRandomNumberService
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<RandomNumberService> _logger;

    public RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger)
    {
      _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Fetches a random number from an external service.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the request.</param>
    /// <returns>An integer representing the random number.</returns>
    public async Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        // Make request to external service
        var response = await _httpClient.GetAsync("random", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
          _logger.LogError("Failed to fetch random number. Status Code: {StatusCode}", response.StatusCode);
          throw new ExternalServiceException("Failed to retrieve a random number from the external service.");
        }

        // Read and parse JSON response
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var jsonResponse = JsonConvert.DeserializeObject<RandomNumberResponse>(responseContent);

        if (jsonResponse?.RandomNumber != null)
        {
          return jsonResponse.RandomNumber.Value;
        }

        // Log failure and throw if parsing is unsuccessful
        _logger.LogError("Invalid response format: {ResponseContent}", responseContent);
        throw new ExternalServiceException("Invalid response format received from external service.");
      }
      catch (HttpRequestException ex)
      {
        _logger.LogError(ex, "Error occurred while fetching random number from external service.");
        throw new ExternalServiceException("Failed to retrieve random number due to network issues.");
      }
      catch (TaskCanceledException)
      {
        _logger.LogWarning("Random number request was canceled.");
        throw; // Let it propagate as-is to handle cancellation correctly
      }
      catch (JsonException ex)
      {
        _logger.LogError(ex, "Error parsing JSON response from external service.");
        throw new ExternalServiceException("Failed to parse external service response.");
      }
    }

    private class RandomNumberResponse
    {
      [JsonProperty("random_number")]
      public int? RandomNumber { get; set; }
    }
  }
}
