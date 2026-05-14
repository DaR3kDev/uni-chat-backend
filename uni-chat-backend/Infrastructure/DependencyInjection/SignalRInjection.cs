namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class SignalRInjection
{
    public static void AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR();
    }
}