using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class ConversationRepository(IMongoCollections mongoCollections) : IConversationRepository
{
    private readonly IMongoCollection<Conversation> _conversations = mongoCollections.Conversations;

    public async Task CreateAsync(Conversation conversation) =>
        await _conversations.InsertOneAsync(conversation);

    public async Task<Conversation?> GetByIdAsync(Guid id) =>
        await _conversations
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync();
}