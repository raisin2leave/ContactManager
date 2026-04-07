using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppCore.Exceptions; // Ensure this matches your exceptions namespace

namespace WebApi.Middleware;

public class ProblemDetailsExceptionHandler(ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        // Define the error details
        var problemDetails = new ProblemDetails
        {
            Status = exception switch
            {
                ContactNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            },
            Title = "An error occurred while processing your request",
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // Send the JSON response
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}