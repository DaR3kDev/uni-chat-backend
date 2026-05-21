using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Login.Interfaces;

public interface IUserLoginSessionCache
{
    Task CreateSessionAsync(User user);

    Task MarkOnlineAsync(Guid userId);
}
