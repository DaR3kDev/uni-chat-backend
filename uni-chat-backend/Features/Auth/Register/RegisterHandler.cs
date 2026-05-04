using MediatR;
using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Features.Auth.Register;

public class RegisterHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService,
    IConnectionMultiplexer redis
) : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLower();
        var username = request.Username.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            throw new ConflictException("El correo electrónico ya está en uso");

        var existingUsername = await _userRepository.GetByUsernameAsync(username);

        if (existingUsername is not null)
            throw new ConflictException("El nombre de usuario ya está en uso");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = request.Phone,
            Email = email,
            Username = username,
            IsOnline = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.CreateAsync(refreshToken);

        var db = _redis.GetDatabase();

        var sessionData = new
        {
            user.Id,
            user.Username,
            user.Email,
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