using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence;

namespace uni_chat_backend.Infrastructure.Persistence.Collections;

public class MongoCollections(MongoContext context) : IMongoCollections
{
    public IMongoCollection<User> Users { get; set; } = context.Database.GetCollection<User>(MongoCollectionNames.Users);
    public IMongoCollection<Message> Messages { get; set; } = context.Database.GetCollection<Message>(MongoCollectionNames.Messages);
    public IMongoCollection<Conversation> Conversations { get; set; } = context.Database.GetCollection<Conversation>(MongoCollectionNames.Conversations);
}
