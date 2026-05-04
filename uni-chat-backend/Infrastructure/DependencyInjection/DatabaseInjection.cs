using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StackExchange.Redis;
using uni_chat_backend.Infrastructure.Persistence;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Persistence.Indexes.Initialization;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class DatabaseInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Mongo serialization
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        services.Configure<MongoSettings>(configuration.GetSection("Mongo"));

        // Mongo Client
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Mongo ConnectionString is not configured");

            return new MongoClient(settings.ConnectionString);
        });

        services.AddSingleton<MongoContext>();
        services.AddSingleton<IMongoCollections, MongoCollections>();
        services.AddHostedService<MongoIndexesInitializerService>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redis = configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(redis))
                throw new InvalidOperationException("Redis ConnectionString is not configured");

            return ConnectionMultiplexer.Connect(redis);
        });

        return services;
    }
}