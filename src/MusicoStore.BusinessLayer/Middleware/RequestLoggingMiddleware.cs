using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.BusinessLayer.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly string _applicationTag;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, string applicationTag)
    {
        _next = next;
        _logger = logger;
        _applicationTag = applicationTag;
    }

    public async Task InvokeAsync(HttpContext context, ILoggingRepository loggingRepository)
    {
        var path = context.Request.Path;

         if (path.StartsWithSegments("/favicon.ico") || path.StartsWithSegments("/swagger")) 
        {
            await _next(context);
            return;
        }

        var now = DateTime.UtcNow;
        var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown"; 
        var queryString = context.Request.QueryString;

        var requestLogMessage =
            $"{_applicationTag} [{now:yyyy-MM-dd HH:mm:ss}] Incoming {context.Request.Method} {path}{queryString} from {remoteIp}"; 

        try
        {
            _logger.LogInformation(requestLogMessage);

            await loggingRepository.AddAsync(
                context.Request.Method,
                $"{path}{queryString}",
                $"{_applicationTag} Incoming request",
                context.RequestAborted
            );
        }
        catch (Exception ex)
        {
             _logger.LogWarning(ex, $"Failed to log incoming {_applicationTag} request.");
        }

        await _next(context);

        var responseNow = DateTime.UtcNow;
        var responseLogMessage =
            $"{_applicationTag} [{responseNow:yyyy-MM-dd HH:mm:ss}] Response for {context.Request.Method} {path} with status code {context.Response.StatusCode}";

        try
        {
            _logger.LogInformation(responseLogMessage);

            await loggingRepository.AddAsync(
                context.Request.Method,
                $"{path}{queryString}",
                $"{_applicationTag} Outgoing response - status: {context.Response.StatusCode}",
                context.RequestAborted
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Failed to log outgoing {_applicationTag} response.");
        }
    }
}
