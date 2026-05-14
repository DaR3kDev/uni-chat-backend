using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class MessageRepository(IMongoCollections mongoCollections) : IMessageRepository
{
    private readonly IMongoCollection<Message> _messages = mongoCollections.Messages;
    private readonly IMongoCollections _mongoCollections = mongoCollections;

    public async Task CreateAsync(Message message)
    {
        await _messages.InsertOneAsync(message);
    }

    public async Task<Message?> GetByIdAsync(Guid messageId)
    {
        return await _messages
            .Find(m => m.Id == messageId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int limit = 50)
    {
        var messages = await _messages
            .Find(m =>
                m.ConversationId == conversationId &&
                !m.IsDeleted
            )
            .SortByDescending(m => m.CreatedAt)
            .Limit(limit)
            .ToListAsync();

        return [.. messages.OrderBy(m => m.CreatedAt)];
    }

    public async Task MarkAsDeletedAsync(Guid messageId)
    {
        await _messages.UpdateOneAsync(
            m => m.Id == messageId,
            Builders<Message>.Update.Set(m => m.IsDeleted, true)
        );
    }

    public async Task<int> MarkConversationAsReadAsync(Guid conversationId, Guid userId)
    {
        var messages = await _messages
            .Find(m => m.ConversationId == conversationId && !m.IsDeleted)
            .ToListAsync();

        var readsCollection = _mongoCollections.MessageReads;

        var reads = messages.Select(m => new MessageRead
        {
            MessageId = m.Id,
            UserId = userId,
            ReadAt = DateTime.UtcNow
        }).ToList();

        if (reads.Count == 0)
            return 0;

        await readsCollection.InsertManyAsync(reads);

        return reads.Count;
    }

    public async Task UpdateStatusAsync(Guid messageId, MessageStatus status)
    {
        var update = Builders<Message>.Update
            .Set(x => x.Status, status);

        await _messages.UpdateOneAsync(
            x => x.Id == messageId,
            update
        );
    }
}