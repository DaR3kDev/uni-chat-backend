using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class ConversationsIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<Conversation>("conversations");

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Conversation>(
                Builders<Conversation>.IndexKeys
                    .Descending(x => x.LastMessageAt)
            )
        );

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Conversation>(
                Builders<Conversation>.IndexKeys
                    .Ascending("Participants.UserId")
            )
        );
    }
}