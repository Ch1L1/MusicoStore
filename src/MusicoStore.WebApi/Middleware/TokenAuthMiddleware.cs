using System.Net;
using Microsoft.AspNetCore.Http;

namespace MusicoStore.WebApi.Middleware;

public class TokenAuthMiddleware
{
    private readonly RequestDelegate _next;
    private const string ExpectedToken = "simple-token";

    public TokenAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/swagger") || path.StartsWith("/health") || path.StartsWith("/_framework"))
        {
            await _next(context);
            return;
        }

        // Read token
        string? token = null;
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var headerValue = authHeader.ToString();
            if (headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = headerValue.Substring("Bearer ".Length).Trim();
            }
        }

        if (string.IsNullOrEmpty(token) && context.Request.Headers.TryGetValue("X-Access-Token", out var altHeader))
        {
            token = altHeader.ToString();
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Missing access token.");
            return;
        }

        if (!string.Equals(token, ExpectedToken, StringComparison.Ordinal))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await context.Response.WriteAsync("Invalid access token.");
            return;
        }

        await _next(context);
    }
}
