using Serilog.Context;

namespace uni_chat_backend.API.Middleware;

public sealed class RequestIdMiddleware(
    RequestDelegate next,
    ILogger<RequestIdMiddleware> logger
)
{
    private const string HeaderName = "X-Request-Id";
    private const string ItemKey = "RequestId";

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId =
            context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(requestId)) requestId = Guid.NewGuid().ToString("N");

        context.Items[ItemKey] = requestId;

        context.Response.Headers[HeaderName] = requestId;

        using var scope = logger.BeginScope(
            new Dictionary<string, object>
            {
                ["RequestId"] = requestId
            }
        );

        using var logContext =
            LogContext.PushProperty("RequestId", requestId);

        logger.LogInformation(
            "Incoming request {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );

        await next(context);

        logger.LogInformation(
            "Response completed with status code {StatusCode}",
            context.Response.StatusCode
        );
    }
}
