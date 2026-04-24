using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IMessageRepository
{
    Task CreateAsync(Message message);
    Task<List<Message>> GetByConversationAsync(Guid conversationId, int limit = 50);
}

