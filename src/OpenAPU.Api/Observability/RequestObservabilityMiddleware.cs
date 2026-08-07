using System.Diagnostics;

namespace OpenAPU.Api.Observability;

public sealed class RequestObservabilityMiddleware
{
    public const string CorrelationHeader =
        "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestObservabilityMiddleware> _logger;

    public RequestObservabilityMiddleware(
        RequestDelegate next,
        ILogger<RequestObservabilityMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            ResolveCorrelationId(context);

        context.TraceIdentifier = correlationId;
        context.Response.Headers[CorrelationHeader] =
            correlationId;

        using var scope = _logger.BeginScope(
            new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            });

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled request error. {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            throw;
        }
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    private static string ResolveCorrelationId(
        HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(
                CorrelationHeader,
                out var supplied) &&
            !string.IsNullOrWhiteSpace(supplied))
        {
            return supplied.ToString();
        }

        return Guid.NewGuid().ToString("N");
    }
}
