using System.Security.Cryptography;
using System.Text;

namespace OpenAPU.Api.Security;

public sealed class AdminAccessMiddleware
{
    public const string HeaderName =
        "X-OpenAPU-Admin-Key";

    private static readonly string[] ProtectedPaths =
    [
        "/database/backup",
        "/database/restore",
        "/support/diagnostics"
    ];

    private readonly RequestDelegate _next;
    private readonly AdminAccessOptions _options;

    public AdminAccessMiddleware(
        RequestDelegate next,
        AdminAccessOptions options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_options.Enabled ||
            !IsProtected(context.Request.Path))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(
                HeaderName,
                out var supplied) ||
            !Matches(supplied.ToString(), _options.Key!))
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            context.Response.ContentType =
                "application/problem+json";

            await context.Response.WriteAsJsonAsync(new
            {
                status = StatusCodes.Status401Unauthorized,
                title = "Admin key required."
            });

            return;
        }

        await _next(context);
    }

    private static bool IsProtected(
        PathString path)
    {
        return ProtectedPaths.Any(
            protectedPath =>
                path.StartsWithSegments(
                    protectedPath,
                    StringComparison.OrdinalIgnoreCase));
    }

    private static bool Matches(
        string supplied,
        string expected)
    {
        var suppliedBytes =
            Encoding.UTF8.GetBytes(supplied);

        var expectedBytes =
            Encoding.UTF8.GetBytes(expected);

        return suppliedBytes.Length ==
                expectedBytes.Length &&
            CryptographicOperations.FixedTimeEquals(
                suppliedBytes,
                expectedBytes);
    }
}
