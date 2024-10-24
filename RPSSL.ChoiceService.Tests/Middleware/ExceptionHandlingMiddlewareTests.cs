using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RPSSL.ChoiceService.Exceptions;
using RPSSL.ChoiceService.Middlewares;
using System.Net;
using System.Text.Json;

namespace RPSSL.ChoiceService.Tests.Middleware;
public class ExceptionHandlingMiddlewareTests
{
  private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;

  public ExceptionHandlingMiddlewareTests()
  {
    _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
  }

  private DefaultHttpContext CreateHttpContext()
  {
    var context = new DefaultHttpContext();
    context.Response.Body = new MemoryStream();
    return context;
  }

  [Fact]
  public async Task Middleware_ShouldReturnBadRequest_ForInvalidChoiceException()
  {
    // Arrange
    var middleware = new ExceptionHandlingMiddleware(
        async (innerHttpContext) =>
        {
          throw new InvalidChoiceException("Invalid choice selected");
        }, _mockLogger.Object);

    var context = CreateHttpContext();

    // Act
    await middleware.InvokeAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    context.Response.ContentType.Should().Be("application/json");

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
    responseJson.Should().ContainKey("Message");

    // Clean up and ensure comparison is correct
    var actualMessage = responseJson["Message"].ToString().Trim();
    actualMessage.Should().Be("Invalid choice selected",
        $"Expected 'Invalid choice selected' but found '{actualMessage}'");
  }





  [Fact]
  public async Task Middleware_ShouldReturnInternalServerError_ForGenericException()
  {
    // Arrange
    var middleware = new ExceptionHandlingMiddleware(
        async (innerHttpContext) =>
        {
          throw new Exception("Something went wrong");
        }, _mockLogger.Object);

    var context = CreateHttpContext();

    // Act
    await middleware.InvokeAsync(context);

    // Assert
    context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    context.Response.ContentType.Should().Be("application/json");

    context.Response.Body.Seek(0, SeekOrigin.Begin);
    var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();

    var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
    responseJson.Should().ContainKey("Message");

    // Clean up and ensure comparison is correct
    var actualMessage = responseJson["Message"].ToString().Trim();
    actualMessage.Should().Be("Something went wrong",
        $"Expected 'Something went wrong' but found '{actualMessage}'");
  }
}
