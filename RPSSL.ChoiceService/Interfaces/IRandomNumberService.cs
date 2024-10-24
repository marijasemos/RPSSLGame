namespace RPSSL.ChoiceService.Interfaces;

/// <summary>
/// Interface for a service that retrieves a random number from an external source.
/// </summary>
public interface IRandomNumberService
{
  /// <summary>
  /// Asynchronously fetches a random number from an external service.
  /// </summary>
  /// <param name="cancellationToken">A token to cancel the operation.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains the random number.</returns>
  Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default);
}


