using AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

public class ProblemDetailsExceptionHandler(
    ProblemDetailsFactory factory, ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ContactNotFoundException)
        {
            logger.LogInformation($"Exception '{exception.Message}' handled!");
            
            var problem = factory.CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound, // Changed to 404 as it is more standard for "Not Found"
                title: "Contact service error!",
                detail: exception.Message
            );

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        return false;
    }
}