using Scalar.AspNetCore;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class DocsConfiguration
{
    public static void UseApiDocs(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}
