using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class MessageDeliveryIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<MessageDelivery>("message_delivery");

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<MessageDelivery>(
                Builders<MessageDelivery>.IndexKeys
                    .Ascending(x => x.MessageId)
                    .Ascending(x => x.UserId)
            )
        );
    }
}