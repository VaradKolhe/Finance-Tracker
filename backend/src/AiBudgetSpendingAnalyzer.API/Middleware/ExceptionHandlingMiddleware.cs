using AiBudgetSpendingAnalyzer.Application.Common.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace AiBudgetSpendingAnalyzer.API.Middleware;

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
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                message = "Validation failed.",
                errors = validationException.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (AppException appException)
        {
            context.Response.StatusCode = appException.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = appException.Message }));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "An unexpected server error occurred." }));
        }
    }
}
