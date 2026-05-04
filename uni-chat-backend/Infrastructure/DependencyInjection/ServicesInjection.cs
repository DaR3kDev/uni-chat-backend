using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;
using uni_chat_backend.Infrastructure.Services;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class ServicesInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // =========================================================
        // SECURITY SERVICES
        // =========================================================
        services.AddSingleton<TokenService>();

        services.AddScoped<ICurrentUserService,CurrentUserService>();

        // =========================================================
        // EXTERNAL SERVICES
        // =========================================================
        services.AddSingleton<CloudinaryService>();

        return services;
    }
}