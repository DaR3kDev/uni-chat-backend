using Microsoft.Extensions.Options;
using uni_chat_backend.Infrastructure.Configuration;
using uni_chat_backend.Infrastructure.Persistence;
using uni_chat_backend.Infrastructure.Settings;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class ConfigurationInjection
{
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<RefreshTokenSettings>(configuration.GetSection("RefreshToken"));
        services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RefreshTokenSettings>>().Value);
    }
}
