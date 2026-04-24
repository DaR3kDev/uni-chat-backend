using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Configurations;

public static class MessageIndexes
{
    public static void Create(IMongoDatabase db)
    {
        var messages = db.GetCollection<Message>("messages");

        messages.Indexes.CreateMany(
        [
            new CreateIndexModel<Message>(
                Builders<Message>.IndexKeys
                    .Ascending(x => x.ConversationId)
                    .Ascending(x => x.CreatedAt)
            )
        ]);
    }
}

