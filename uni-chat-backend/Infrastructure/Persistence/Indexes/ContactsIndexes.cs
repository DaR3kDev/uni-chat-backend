using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Indexes;

public static class ContactsIndexes
{
    public static async Task Create(IMongoDatabase db)
    {
        var collection = db.GetCollection<Contact>("contacts");

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Contact>(
                Builders<Contact>.IndexKeys
                    .Ascending(x => x.OwnerUserId)
                    .Ascending(x => x.ContactUserId),
                new CreateIndexOptions { Unique = true }
            )
        );

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<Contact>(
                Builders<Contact>.IndexKeys
                    .Ascending(x => x.OwnerUserId)
            )
        );
    }
}