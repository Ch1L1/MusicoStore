using MusicoStore.Domain.Interfaces.Repository;

namespace MusicoStore.WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly ILoggingRepository _loggingRepository;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        ILoggingRepository loggingRepository)
    {
        _next = next;
        _logger = logger;
        _loggingRepository = loggingRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var now = DateTime.UtcNow;

        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/favicon.ico"))
        {
            await _next(context);
            return;
        }

        var requestLogMessage =
            $"[{now:yyyy-MM-dd HH:mm:ss}] Incoming {context.Request.Method} {context.Request.Path}{context.Request.QueryString} from {context.Connection.RemoteIpAddress}";

        _logger.LogInformation(requestLogMessage);

        await _loggingRepository.AddAsync(
            context.Request.Method,
            $"{context.Request.Path}{context.Request.QueryString}",
            "Incoming request",
            CancellationToken.None
        );

        await _next(context);

        var responseNow = DateTime.UtcNow;

        var responseLogMessage =
            $"[{responseNow:yyyy-MM-dd HH:mm:ss}] Response for {context.Request.Method} {context.Request.Path} with status code {context.Response.StatusCode}";

        _logger.LogInformation(responseLogMessage);

        await _loggingRepository.AddAsync(
            context.Request.Method,
            $"{context.Request.Path}{context.Request.QueryString}",
            $"Outgoing response - status: {context.Response.StatusCode}",
            CancellationToken.None
        );
    }
}
