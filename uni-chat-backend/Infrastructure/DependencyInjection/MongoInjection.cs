using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using uni_chat_backend.Infrastructure.Persistence;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Persistence.Indexes.Initialization;

namespace uni_chat_backend.Infrastructure.DependencyInjection;

public static class MongoInjection
{
    public static void AddMongoDatabase(this IServiceCollection services)
    {
        // =========================================================
        // BSON SERIALIZATION
        // =========================================================
        BsonSerializer.RegisterSerializer(
            new GuidSerializer(GuidRepresentation.Standard)
        );

        // =========================================================
        // MONGO CLIENT
        // =========================================================
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp
                .GetRequiredService<IOptions<MongoSettings>>()
                .Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException(
                    "Mongo ConnectionString is not configured"
                );

            return new MongoClient(settings.ConnectionString);
        });

        // =========================================================
        // MONGO CONTEXT
        // =========================================================
        services.AddSingleton<MongoContext>();

        services.AddSingleton<IMongoCollections, MongoCollections>();

        services.AddHostedService<MongoIndexesInitializerService>();
    }
}