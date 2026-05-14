using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Refresh.Interfaces;

public interface IRefreshSessionCache
{
    Task CreateSessionAsync(User user);

    Task MarkOnlineAsync(Guid userId);
}