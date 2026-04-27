using uni_chat_backend.API.Middleware;

namespace uni_chat_backend.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomException(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}

