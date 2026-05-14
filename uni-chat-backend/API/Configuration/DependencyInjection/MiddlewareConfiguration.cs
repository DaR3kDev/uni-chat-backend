using uni_chat_backend.API.Extensions;
using uni_chat_backend.API.Middleware;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class MiddlewareConfiguration
{
    public static void UseApiMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestIdMiddleware>();

        app.MapEndpoints();

        app.UseCustomMiddlewares();
    }
}