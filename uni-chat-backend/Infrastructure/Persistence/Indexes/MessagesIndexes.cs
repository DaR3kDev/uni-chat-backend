using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class MessagesIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<Message>("messages");

        // chat por conversación 
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys
                    .Ascending(x => x.ConversationId)
                    .Descending(x => x.CreatedAt)
            )
        );

        // mensajes por usuario
        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys.Ascending(x => x.SenderId)
            )
        );
    }
}