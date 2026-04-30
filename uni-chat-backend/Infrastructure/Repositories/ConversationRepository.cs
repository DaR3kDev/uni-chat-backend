using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class ConversationRepository(IMongoCollections mongoCollections) : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversations = mongoCollections.Conversations;
    private readonly IMongoCollection<User> _users = mongoCollections.Users;

    public async Task CreateAsync(Conversation conversation) =>
        await _conversations.InsertOneAsync(conversation);

    public async Task<Conversation?> GetByIdAsync(Guid id) =>
       await _conversations
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync();

    public Task<List<Conversation>> GetUserConversationsAsync(Guid userId) =>
        _conversations
            .Find(c => c.Participants.Any(p => p.UserId == userId))
            .SortByDescending(c => c.LastMessageAt)
            .ToListAsync();

    public async Task UpdateLastMessageAsync(Guid conversationId, DateTime date) =>
        await _conversations.UpdateOneAsync(
            c => c.Id == conversationId,
            Builders<Conversation>.Update.Set(c => c.LastMessageAt, date)
        );

    public async Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _conversations
            .Find(c => c.Id == conversationId)
            .FirstOrDefaultAsync();

        return conversation?.Participants
            .Any(p => p.UserId == userId && !p.IsBanned) ?? false;
    }

    public async Task SetUserOnlineAsync(Guid userId) =>
        await _users.UpdateOneAsync(
            u => u.Id == userId,
            Builders<User>.Update.Set(u => u.IsOnline, true)
        );

    public async Task SetUserOfflineAsync(Guid userId) =>
        await _users.UpdateOneAsync(
            u => u.Id == userId,
            Builders<User>.Update
                .Set(u => u.IsOnline, false)
                .Set(u => u.LastSeen, DateTime.UtcNow)
        );

    public async Task<string> GetEncryptionKeyAsync(Guid conversationId)
    {
        var conversation = await _conversations
            .Find(c => c.Id == conversationId)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException("Conversación no existe");

        if (string.IsNullOrWhiteSpace(conversation.EncryptionKey))
            throw new InvalidOperationException("Conversación sin key de cifrado");

        return conversation.EncryptionKey;
    }

    public async Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2) =>
         await _conversations
            .Find(c =>
                !c.IsGroup &&
                c.Participants != null &&
                c.Participants.Any(p => p.UserId == userId1) &&
                c.Participants.Any(p => p.UserId == userId2) &&
                c.Participants.Count == 2
            )
            .FirstOrDefaultAsync();


}