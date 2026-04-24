using MongoDB.Driver;

namespace uni_chat_backend.Infrastructure.Persistence;

public class MongoContext
{
    public IMongoDatabase Database { get; }

    public MongoContext(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("MongoDb");
        var databaseName = config["MongoDb:Database"];

        var client = new MongoClient(connectionString);

        Database = client.GetDatabase(databaseName);
    }
}
