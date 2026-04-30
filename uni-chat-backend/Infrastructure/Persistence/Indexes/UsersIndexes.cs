using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class UsersIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<User>("users");

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions { Unique = true }
            )
        );

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.Phone),
                new CreateIndexOptions { Unique = true, Sparse = true }
            )
        );
    }
}