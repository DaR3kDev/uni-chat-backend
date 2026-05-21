using StackExchange.Redis;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class RedisInjection
{
    public static void AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionString =
                configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Redis ConnectionString is not configured"
                );

            return ConnectionMultiplexer.Connect(connectionString);
        });
    }
}
