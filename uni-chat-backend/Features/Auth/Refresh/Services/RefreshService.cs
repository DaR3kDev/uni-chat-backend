using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Auth.Refresh.Interfaces;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Refresh.Services;

public class RefreshService(
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    TokenService tokenService,
    IRefreshTokenValidator validator,
    IRefreshSessionCache sessionCache,
    IRefreshCookieService cookieService,
    ILogger<RefreshService> logger)
{
    public async Task<AuthResponse> RefreshAsync(CancellationToken cancellationToken)
    {
        var context = httpContextAccessor.HttpContext ?? throw new UnauthorizedException("No hay contexto HTTP");

        var requestId = context.Items["RequestId"]?.ToString() ?? "desconocido";

        logger.LogInformation("[{RequestId}] Iniciando renovación de tokens", requestId);

        var token = context.Request.Cookies["refreshToken"]?.Trim() ??
                    throw new UnauthorizedException("Refresh token no encontrado");

        logger.LogInformation("[{RequestId}] Validando refresh token", requestId);

        var storedToken = await validator.ValidateAsync(token);

        logger.LogInformation("[{RequestId}] Refresh token validado correctamente para el usuario: {UserId}", requestId,
            storedToken.UserId);

        var user = await userRepository.GetByIdAsync(storedToken.UserId) ??
                   throw new NotFoundException("Usuario no encontrado");

        logger.LogInformation("[{RequestId}] Generando nuevos tokens para el usuario: {UserId}", requestId, user.Id);

        var newAccessToken = tokenService.GenerateAccessToken(user);

        var newRefreshToken = tokenService.GenerateRefreshToken(user.Id);

        logger.LogInformation("[{RequestId}] Revocando tokens anteriores del usuario: {UserId}", requestId, user.Id);

        await refreshTokenRepository.RevokeAllByUserIdAsync(user.Id);

        await refreshTokenRepository.CreateAsync(newRefreshToken);

        logger.LogInformation("[{RequestId}] Creando sesión y marcando usuario en línea: {UserId}", requestId, user.Id);

        await sessionCache.CreateSessionAsync(user);

        await sessionCache.MarkOnlineAsync(user.Id);

        cookieService.SetRefreshToken(context, newRefreshToken.Token);

        logger.LogInformation("[{RequestId}] Cookie de refresh token actualizada correctamente", requestId);

        logger.LogInformation("[{RequestId}] Renovación de tokens completada correctamente para el usuario: {UserId}",
            requestId, user.Id);

        return new AuthResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token };
    }
}
