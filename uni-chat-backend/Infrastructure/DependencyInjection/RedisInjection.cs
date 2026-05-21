using Microsoft.Extensions.Options;
using StackExchange.Redis;
using uni_chat_backend.Infrastructure.Settings;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class RedisInjection
{
    public static void AddRedis(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;

            var connectionString = settings.ConnectionString;

            return string.IsNullOrWhiteSpace(connectionString)
                ? throw new InvalidOperationException("Redis ConnectionString is not configured")
                : ConnectionMultiplexer.Connect(connectionString.Trim());
        });
    }
}
