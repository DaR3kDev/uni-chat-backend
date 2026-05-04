using MediatR;
using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Features.Auth.Refresh;
public class RefreshHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    TokenService tokenService,
    IHttpContextAccessor httpContextAccessor,
    IConnectionMultiplexer redis
) : IRequestHandler<RefreshCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedException("No hay contexto HTTP");

        var token = context.Request.Cookies["refreshToken"]?.Trim()
            ?? throw new UnauthorizedException("Refresh token no encontrado");

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(token)
            ?? throw new UnauthorizedException("Refresh token inválido");

        if (storedToken.IsRevoked)
            throw new UnauthorizedException("Refresh token revocado");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token expirado");

        var user = await _userRepository.GetByIdAsync(storedToken.UserId)
            ?? throw new NotFoundException("Usuario no encontrado");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);
        await _refreshTokenRepository.CreateAsync(newRefreshToken);
        
        var db = _redis.GetDatabase();

        var sessionData = new
        {
            user.Id,
            user.Username,
            user.Phone,
            RefreshedAt = DateTime.UtcNow
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

        context.Response.Cookies.Append(
            "refreshToken",
            newRefreshToken.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
    }
}