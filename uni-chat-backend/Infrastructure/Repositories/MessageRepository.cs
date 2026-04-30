using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class MessageRepository(IMongoCollections mongoCollections) : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages = mongoCollections.Messages;

    public async Task CreateAsync(Message message) =>
        await _messages.InsertOneAsync(message);

    public async Task<Message?> GetByIdAsync(Guid messageId) =>
        await _messages
            .Find(m => m.Id == messageId)
            .FirstOrDefaultAsync();

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int limit = 50) =>
        await _messages
            .Find(m => m.ConversationId == conversationId && !m.IsDeleted)
            .SortByDescending(m => m.CreatedAt)
            .Limit(limit)
            .ToListAsync();

    public async Task MarkAsDeletedAsync(Guid messageId) =>
        await _messages.UpdateOneAsync(
            m => m.Id == messageId,
            Builders<Message>.Update.Set(m => m.IsDeleted, true)
        );
}