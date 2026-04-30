using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken token);
    Task ReplaceAsync(Guid userId, RefreshToken newToken);
}

