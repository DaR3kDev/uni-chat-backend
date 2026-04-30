using MongoDB.Driver;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class MongoIndexesInitializer
{
    public static async Task Initialize(IMongoDatabase db)
    {
        await Task.WhenAll(
            UsersIndexes.Create(db),
            MessagesIndexes.Create(db),
            ConversationsIndexes.Create(db),
            ContactsIndexes.Create(db),
            RefreshTokensIndexes.Create(db),
            MessageDeliveryIndexes.Create(db),
            MessageReadIndexes.Create(db)
        );
    }
}