using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace UnitConversion.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches unhandled exceptions in the request pipeline
/// and returns standardized <see cref="ProblemDetails"/> responses conforming to RFC 7807.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// JSON serialization options configured for camelCase property naming,
    /// consistent with ASP.NET Core API conventions.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware delegate in the pipeline.</param>
    /// <param name="logger">The logger instance for recording exception details.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware, wrapping the downstream pipeline in exception handling logic.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Bad Request", ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
            await WriteProblemDetailsAsync(context, StatusCodes.Status404NotFound, "Not Found", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    /// <summary>
    /// Writes a standardized <see cref="ProblemDetails"/> JSON response to the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <param name="title">A short, human-readable summary of the problem.</param>
    /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Extensions =
            {
                ["traceId"] = context.TraceIdentifier
            }
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails, JsonOptions);
    }
}
