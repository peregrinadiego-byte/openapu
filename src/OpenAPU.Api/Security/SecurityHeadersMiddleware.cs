namespace OpenAPU.Api.Security;

public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            headers.TryAdd(
                "X-Content-Type-Options",
                "nosniff");

            headers.TryAdd(
                "X-Frame-Options",
                "DENY");

            headers.TryAdd(
                "Referrer-Policy",
                "no-referrer");

            headers.TryAdd(
                "Permissions-Policy",
                "camera=(), microphone=(), geolocation=()");

            headers.TryAdd(
                "Content-Security-Policy",
                "default-src 'self'; " +
                "img-src 'self' data:; " +
                "style-src 'self' 'unsafe-inline'; " +
                "script-src 'self' 'unsafe-inline'; " +
                "connect-src 'self'; " +
                "object-src 'none'; " +
                "base-uri 'self'; " +
                "frame-ancestors 'none';");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
