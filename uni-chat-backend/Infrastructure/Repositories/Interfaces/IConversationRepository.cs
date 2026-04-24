using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IConversationRepository
{
    Task CreateAsync(Conversation conversation);
    Task<Conversation?> GetByIdAsync(Guid id);
}