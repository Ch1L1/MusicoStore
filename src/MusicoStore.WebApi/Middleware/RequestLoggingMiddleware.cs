using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MusicoStore.WebApi.Middleware;

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
        _logger.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Received {context.Request.Method} request at {context.Request.Path}{context.Request.QueryString} from {context.Connection.RemoteIpAddress}");

        await _next(context);

        _logger.LogInformation($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] Completed {context.Request.Method} request at {context.Request.Path} with status code {context.Response.StatusCode}");
    }
}
