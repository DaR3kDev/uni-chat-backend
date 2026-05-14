using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Refresh.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Auth.Refresh.Validators;

public class RefreshTokenValidator(IRefreshTokenRepository refreshTokenRepository) : IRefreshTokenValidator
{
    public async Task<RefreshToken> ValidateAsync(string token)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(token) ??
                          throw new UnauthorizedException("Refresh token inválido");

        if (storedToken.IsRevoked) throw new UnauthorizedException("Refresh token revocado");

        return storedToken.ExpiresAt < DateTime.UtcNow
            ? throw new UnauthorizedException("Refresh token expirado")
            : storedToken;
    }
}