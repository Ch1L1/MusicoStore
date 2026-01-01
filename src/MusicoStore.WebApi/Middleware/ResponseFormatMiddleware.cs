namespace MusicoStore.WebApi.Middleware;

public class ResponseFormatMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseFormatMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? queryFormat = context.Request.Query["format"].FirstOrDefault()?.ToLowerInvariant();
        string? accept = context.Request.Headers.Accept.ToString();

        bool wantsXml = queryFormat == "xml" ||
                        (string.IsNullOrEmpty(queryFormat) && accept.Contains("xml", StringComparison.OrdinalIgnoreCase));

        // index 0 => JSON, 1 => XML
        int index = wantsXml ? 1 : 0;
        var acceptValues = new[] { "application/json", "application/xml" };
        context.Request.Headers["Accept"] = acceptValues[index % acceptValues.Length];

        await _next(context);
    }
}
