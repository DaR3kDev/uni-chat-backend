using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Register.Interfaces;

public interface IUserSessionCache
{
    Task CreateSessionAsync(User user);
    Task MarkOnlineAsync(Guid userId);
}
