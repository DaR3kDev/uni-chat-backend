using Scalar.AspNetCore;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class DocsConfiguration
{
    public static void UseApiDocs(this WebApplication app)
    {
         var enableDocs =
            app.Environment.IsDevelopment() ||
            Environment.GetEnvironmentVariable("ENABLE_DOCS") == "true";

        if (!enableDocs) return;

        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}
