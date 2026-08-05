using Microsoft.AspNetCore.Http;
using OpenAPU.Api.Security;

namespace OpenAPU.Api.Tests;

public sealed class AdminAccessMiddlewareTests
{
    [Fact]
    public async Task Public_route_is_allowed()
    {
        var called = false;

        var middleware = new AdminAccessMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            new AdminAccessOptions("secret"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/health";

        await middleware.InvokeAsync(context);

        Assert.True(called);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Protected_route_requires_key()
    {
        var called = false;

        var middleware = new AdminAccessMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            new AdminAccessOptions("secret"));

        var context = new DefaultHttpContext();
        context.Request.Path = "/database/backup";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.False(called);
        Assert.Equal(
            StatusCodes.Status401Unauthorized,
            context.Response.StatusCode);
    }

    [Fact]
    public async Task Protected_route_accepts_correct_key()
    {
        var called = false;

        var middleware = new AdminAccessMiddleware(
            _ =>
            {
                called = true;
                return Task.CompletedTask;
            },
            new AdminAccessOptions("secret"));

        var context = new DefaultHttpContext();
        context.Request.Path =
            "/support/diagnostics/download";

        context.Request.Headers[
            AdminAccessMiddleware.HeaderName] =
            "secret";

        await middleware.InvokeAsync(context);

        Assert.True(called);
        Assert.Equal(200, context.Response.StatusCode);
    }
}
