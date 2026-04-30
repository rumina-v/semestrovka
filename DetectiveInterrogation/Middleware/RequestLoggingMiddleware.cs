using Serilog;

namespace DetectiveInterrogation.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString;

        _logger.LogInformation("Request started: {Method} {Path}{QueryString}", method, path, queryString);

        await _next(context);

        var duration = DateTime.UtcNow - startTime;
        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("Request completed: {Method} {Path}{QueryString} - Status: {StatusCode} - Duration: {Duration}ms",
            method, path, queryString, statusCode, duration.TotalMilliseconds);
    }
}