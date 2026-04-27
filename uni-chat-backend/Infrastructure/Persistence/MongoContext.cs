using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace uni_chat_backend.Infrastructure.Persistence;

public class MongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(IMongoClient client, IOptions<MongoSettings> options)
    {
        var settings = options.Value;

        if (string.IsNullOrWhiteSpace(settings.Database))
            throw new InvalidOperationException("Mongo Database is not configured");

        Database = client.GetDatabase(settings.Database);
    }
}