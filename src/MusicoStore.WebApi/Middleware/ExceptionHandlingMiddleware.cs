using System.Net;

namespace MusicoStore.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        HttpStatusCode statusCode;

        switch (ex)
        {
            case InvalidOperationException:
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var responseObj = new
        {
            error = ex.Message
        };

        return context.Response.WriteAsJsonAsync(responseObj);
    }
}
