using System.Reflection;

namespace uni_chat_backend.API.Configuration.Middleware;

public static class MediatorConfiguration
{
    public static void AddMediatorConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
    }
}
