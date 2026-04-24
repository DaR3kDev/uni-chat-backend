using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Persistence.Collections;

public interface IMongoCollections
{
    IMongoCollection<User> Users { get; set; }
    IMongoCollection<Message> Messages { get; set; }
    IMongoCollection<Conversation> Conversations { get; set; }
}

