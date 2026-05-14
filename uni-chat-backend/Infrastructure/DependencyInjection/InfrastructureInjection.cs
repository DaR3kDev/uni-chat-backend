using uni_chat_backend.Features.Auth;
using uni_chat_backend.Features.Auth.Login;
using uni_chat_backend.Features.Auth.Logout;
using uni_chat_backend.Features.Auth.Me;
using uni_chat_backend.Features.Auth.Register;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class InfrastructureInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddConfigurations(configuration);
        services.AddMongoDatabase();
        services.AddRedis(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddRepositories();
        services.AddApplicationBehaviors();
        services.AddInfrastructureServices();
        services.AddSignalRServices();


        // =========================================================
        // FEATURES
        // =========================================================
        services.AddAuthFeatures();
    }
}