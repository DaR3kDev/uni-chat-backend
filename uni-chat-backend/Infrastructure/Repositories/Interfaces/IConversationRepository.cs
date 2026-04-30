using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IConversationRepository
{
    Task CreateAsync(Conversation conversation);

    Task<Conversation?> GetByIdAsync(Guid id);

    Task<List<Conversation>> GetUserConversationsAsync(Guid userId);

    Task UpdateLastMessageAsync(Guid conversationId, DateTime date);

    Task<bool> IsUserInConversationAsync(Guid conversationId, Guid userId);

    Task<string> GetEncryptionKeyAsync(Guid conversationId);

    Task SetUserOnlineAsync(Guid userId);

    Task SetUserOfflineAsync(Guid userId);

    Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2);
}