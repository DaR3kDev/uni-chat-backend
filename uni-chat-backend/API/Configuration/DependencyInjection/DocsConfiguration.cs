using Scalar.AspNetCore;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class DocsConfiguration
{
    public static void UseApiDocs(this WebApplication app)
    {
        var enableDocs =
            app.Environment.IsDevelopment() ||
            app.Configuration.GetValue<bool>("EnableDocs");

        if (!enableDocs) return;

        app.MapOpenApi("/openapi/v1.json");

        app.MapScalarApiReference(options =>
        {
            options.WithOpenApiRoutePattern("/openapi/v1.json");
        });
    }
}