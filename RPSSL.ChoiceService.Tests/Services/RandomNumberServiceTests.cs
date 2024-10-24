using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RPSSL.ChoiceService.Exceptions;
using RPSSL.ChoiceService.Services;
using System.Net;

namespace RPSSL.ChoiceService.Tests.Services;
public class RandomNumberServiceTests
{
  private readonly Mock<ILogger<RandomNumberService>> _mockLogger;
  private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

  public RandomNumberServiceTests()
  {
    _mockLogger = new Mock<ILogger<RandomNumberService>>();
    _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
  }

  private RandomNumberService CreateService()
  {
    var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
    {
      BaseAddress = new Uri("https://fakeurl.com")
    };
    return new RandomNumberService(httpClient, _mockLogger.Object);
  }

  [Fact]
  public async Task GetRandomNumberAsync_ShouldReturnRandomNumber_WhenResponseIsSuccessful()
  {
    // Arrange
    _mockHttpMessageHandler.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.OK,
          Content = new StringContent("{\"random_number\": 5}")
        });

    var service = CreateService();

    // Act
    var result = await service.GetRandomNumberAsync();

    // Assert
    result.Should().Be(5);
  }

  [Fact]
  public async Task GetRandomNumberAsync_ShouldThrowExternalServiceException_WhenHttpStatusCodeIsNotSuccess()
  {
    // Arrange
    _mockHttpMessageHandler.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.BadRequest
        });

    var service = CreateService();

    // Act
    Func<Task> act = async () => await service.GetRandomNumberAsync();

    // Assert
    await act.Should().ThrowAsync<ExternalServiceException>()
        .WithMessage("Failed to retrieve a random number from the external service.");
  }

  [Fact]
  public async Task GetRandomNumberAsync_ShouldThrowExternalServiceException_WhenJsonResponseIsInvalid()
  {
    // Arrange
    _mockHttpMessageHandler.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.OK,
          Content = new StringContent("invalid-json")
        });

    var service = CreateService();

    // Act
    Func<Task> act = async () => await service.GetRandomNumberAsync();

    // Assert
    await act.Should().ThrowAsync<ExternalServiceException>()
        .WithMessage("Failed to parse external service response.");
  }
}

