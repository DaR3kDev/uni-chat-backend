using uni_chat_backend.Features.Auth.Login.Interfaces;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Login.Services;

public class LoginService(
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService,
    IUserLoginValidator validator,
    IUserLoginSessionCache sessionCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<LoginService> logger)
{
    public async Task<AuthResponse> LoginAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "desconocido";

        logger.LogInformation("[{RequestId}] Iniciando proceso de inicio de sesión", requestId);

        logger.LogInformation("[{RequestId}] Validando credenciales del usuario", requestId);

        var user = await validator.ValidateAsync(request.Phone);

        logger.LogInformation("[{RequestId}] Usuario autenticado correctamente con ID: {UserId}", requestId, user.Id);

        logger.LogInformation("[{RequestId}] Generando tokens de autenticación", requestId);

        var accessToken = tokenService.GenerateAccessToken(user);

        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        logger.LogInformation("[{RequestId}] Revocando sesiones y tokens anteriores", requestId);

        await refreshTokenRepository.RevokeAsync(user.Id, refreshToken.Token);

        logger.LogInformation("[{RequestId}] Creando sesión del usuario", requestId);

        await sessionCache.CreateSessionAsync(user);

        await sessionCache.MarkOnlineAsync(user.Id);

        logger.LogInformation("[{RequestId}] Usuario marcado como conectado", requestId);

        logger.LogInformation("[{RequestId}] Inicio de sesión completado correctamente para el usuario: {UserId}",
            requestId, user.Id);

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }
}
