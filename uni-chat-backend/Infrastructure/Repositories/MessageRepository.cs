using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class MessageRepository(IMongoCollections mongoCollections) : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages = mongoCollections.Messages;

    public Task CreateAsync(Message message) =>
        _messages.InsertOneAsync(message);

    public Task<List<Message>> GetByConversationAsync(Guid conversationId, int limit = 50) =>
        _messages
            .Find(m => m.ConversationId == conversationId)
            .SortByDescending(m => m.CreatedAt)
            .Limit(limit)
            .ToListAsync();
}