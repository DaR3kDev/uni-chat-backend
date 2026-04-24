using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Configurations;

public static class ConversationIndexes
{
    public static void Create(IMongoDatabase db)
    {
        var conv = db.GetCollection<Conversation>("conversations");

        conv.Indexes.CreateOne(
            new CreateIndexModel<Conversation>(
                Builders<Conversation>.IndexKeys.Descending(x => x.LastMessageAt)
            )
        );
    }
}

