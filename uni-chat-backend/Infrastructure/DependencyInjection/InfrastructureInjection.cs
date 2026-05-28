using uni_chat_backend.Features.Auth;
using uni_chat_backend.Features.Contacts;
using uni_chat_backend.Features.Conversations;
using uni_chat_backend.Features.Messages;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class InfrastructureInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddConfigurations(configuration);
        services.AddMongoDatabase();
        services.AddRedis();
        services.AddJwtAuthentication(configuration);
        services.AddRepositories();
        services.AddApplicationBehaviors();
        services.AddInfrastructureServices();
        services.AddSignalRServices();


        // =========================================================
        // FEATURES
        // =========================================================
        services.AddAuthFeatures();
        services.AddContactFeatures();
        services.AddConversationsFeatures();
        services.AddMessagesFeatures();
    }
}
