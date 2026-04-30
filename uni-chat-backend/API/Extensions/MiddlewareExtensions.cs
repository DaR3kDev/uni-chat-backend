using uni_chat_backend.API.Middleware;

namespace uni_chat_backend.API.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}