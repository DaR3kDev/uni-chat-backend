using MediatR;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using StackExchange.Redis;
using System.Text.Json;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Features.Auth.Login;

public class LoginHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService,
    IConnectionMultiplexer redis
) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByPhoneAsync(request.Phone)
            ?? throw new NotFoundException("Usuario no encontrado");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.RevokeAsync(user.Id, refreshToken.Token);

        var db = redis.GetDatabase();

        var sessionData = new
        {
            user.Id,
            user.Username,
            user.Phone,
            LoggedAt = DateTime.UtcNow
        };

        await db.StringSetAsync(
            $"session:{user.Id}",
            JsonSerializer.Serialize(sessionData),
            TimeSpan.FromHours(1)
        );

        await db.StringSetAsync(
            $"user:{user.Id}:online",
            "true",
            TimeSpan.FromMinutes(30)
        );

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}