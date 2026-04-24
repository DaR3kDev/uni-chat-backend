using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Configurations;

public static class UserIndexes
{
    public static void Create(IMongoDatabase db)
    {
        var users = db.GetCollection<User>("users");

        users.Indexes.CreateOne(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(x => x.Email),
                new CreateIndexOptions { Unique = true }
            )
        );
    }
}

