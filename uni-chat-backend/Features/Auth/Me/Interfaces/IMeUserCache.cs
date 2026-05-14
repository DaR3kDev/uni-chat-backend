using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Me.Interfaces;

public interface IMeUserCache
{
    Task<User?> GetAsync(Guid userId);

    Task SetAsync(User user);
}