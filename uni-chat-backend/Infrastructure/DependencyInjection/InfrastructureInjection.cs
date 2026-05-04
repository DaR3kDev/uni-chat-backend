namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class InfrastructureInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddConfigurations(configuration);
        services.AddMongoDatabase();
        services.AddRedis(configuration);
        services.AddJwtAuthentication(configuration);
        services.AddRepositories();
        services.AddApplicationBehaviors();
        services.AddInfrastructureServices();

        return services;
    }
}