using Scalar.AspNetCore;
using uni_chat_backend.API.Extensions;
using uni_chat_backend.API.Middleware;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class MiddlewareConfiguration
{
    public static WebApplication UseApiMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("CorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<RequestIdMiddleware>();

        app.UseCustomMiddlewares();

        return app;
    }
}