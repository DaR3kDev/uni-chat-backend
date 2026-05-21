using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(Guid id, string? replacedByToken = null);
    Task RevokeAllByUserIdAsync(Guid userId);
}
