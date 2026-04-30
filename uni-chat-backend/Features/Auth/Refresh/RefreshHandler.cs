using MediatR;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Refresh;

public class RefreshHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    TokenService tokenService,
    IHttpContextAccessor httpContextAccessor
) : IRequestHandler<RefreshCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<AuthResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var context = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("No HTTP context");

        var token = context.Request.Cookies["refreshToken"]?.Trim()
            ?? throw new UnauthorizedAccessException("Refresh token no encontrado");

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(token)
            ?? throw new UnauthorizedAccessException("Refresh token inválido");

        if (storedToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token revocado");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expirado");

        var user = await _userRepository.GetByIdAsync(storedToken.UserId)
            ?? throw new UnauthorizedAccessException("Usuario no encontrado");

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.RevokeAsync(
            storedToken.Id,
            newRefreshToken.Token
        );

        await _refreshTokenRepository.CreateAsync(newRefreshToken);

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