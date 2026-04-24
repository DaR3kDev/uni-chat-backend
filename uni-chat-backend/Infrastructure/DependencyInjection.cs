using MongoDB.Driver;
using uni_chat_backend.Infrastructure.Persistence;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Services;

namespace uni_chat_backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        //MongoClient 
        services.AddSingleton<IMongoClient>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            return new MongoClient(configuration.GetConnectionString("MongoDb"));
        });

        // Contexto y colecciones
        services.AddSingleton<MongoContext>();
        services.AddSingleton<IMongoCollections, MongoCollections>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();

        // Services
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}