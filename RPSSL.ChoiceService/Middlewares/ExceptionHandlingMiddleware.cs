using Newtonsoft.Json;
using RPSSL.ChoiceService.Exceptions;
using System.Net;

namespace RPSSL.ChoiceService.Middlewares;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context); // Call the next middleware
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    _logger.LogError(exception, "An unhandled exception occurred.");

    var statusCode = exception switch
    {
      InvalidChoiceException => (int)HttpStatusCode.BadRequest,
      ExternalServiceException => (int)HttpStatusCode.ServiceUnavailable,
      _ => (int)HttpStatusCode.InternalServerError
    };

    var response = new
    {
      StatusCode = statusCode,
      Message = exception.Message,
      ErrorType = exception.GetType().Name
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
  }
}
