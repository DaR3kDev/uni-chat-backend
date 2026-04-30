using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class RefreshTokensIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<RefreshToken>("refresh_tokens");

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(x => x.Token),
                new CreateIndexOptions { Unique = true }
            )
        );

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<RefreshToken>(
                Builders<RefreshToken>.IndexKeys.Ascending(x => x.UserId)
            )
        );
    }
}