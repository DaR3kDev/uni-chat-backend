namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class SignalRInjection
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR();

        return services;
    }
}
