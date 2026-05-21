using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Refresh.Interfaces;

public interface IRefreshTokenValidator
{
    Task<RefreshToken> ValidateAsync(string token);
}
