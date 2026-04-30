using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IMessageRepository
{
    Task CreateAsync(Message message);

    Task<Message?> GetByIdAsync(Guid messageId);

    Task<List<Message>> GetByConversationIdAsync(Guid conversationId, int limit = 50);

    Task MarkAsDeletedAsync(Guid messageId);
}