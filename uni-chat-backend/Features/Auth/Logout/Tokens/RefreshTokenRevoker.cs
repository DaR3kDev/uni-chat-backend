using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Features.Auth.Logout.Interfaces;

namespace uni_chat_backend.Features.Auth.Logout.Tokens;

public class RefreshTokenRevoker(
    IRefreshTokenRepository refreshTokenRepository
) : IRefreshTokenRevoker
{
    public async Task RevokeAsync(
        string refreshToken
    )
    {
        var tokenEntity =
            await refreshTokenRepository
                .GetByTokenAsync(refreshToken);

        if (tokenEntity is null)
            return;

        if (tokenEntity.IsRevoked)
            return;

        await refreshTokenRepository
            .RevokeAsync(tokenEntity.Id);
    }
}