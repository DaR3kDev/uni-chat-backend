using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Collections;

public interface IMongoCollections
{
    IMongoCollection<User> Users { get; }
    IMongoCollection<Message> Messages { get; }
    IMongoCollection<Conversation> Conversations { get; }
    IMongoCollection<RefreshToken> RefreshTokens { get; }
    IMongoCollection<Contact> Contacts { get; }
    IMongoCollection<MessageDelivery> MessageDeliveries { get; }
    IMongoCollection<MessageRead> MessageReads { get; }
}

