using MongoDB.Driver;

namespace uni_chat_backend.Infrastructure.Persistence.Configurations;

public static class MongoIndexes
{
    public static void Create(IMongoDatabase db)
    {
        UserIndexes.Create(db);
        MessageIndexes.Create(db);
        ConversationIndexes.Create(db);
    }
}

