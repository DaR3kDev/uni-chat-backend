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
    ILogger<LoginService> logger)
{
    public async Task<AuthResponse> LoginAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando proceso de inicio de sesión");

        logger.LogInformation("Validando credenciales del usuario");

        var user = await validator.ValidateAsync(request.Phone);

        logger.LogInformation("Usuario autenticado correctamente con ID: {UserId}", user.Id);

        logger.LogInformation("Generando tokens de autenticación");

        var accessToken = tokenService.GenerateAccessToken(user);

        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        logger.LogInformation("Revocando sesiones y tokens anteriores");

        await refreshTokenRepository.RevokeAsync(user.Id, refreshToken.Token);

        logger.LogInformation("Creando sesión del usuario");

        await sessionCache.CreateSessionAsync(user);

        await sessionCache.MarkOnlineAsync(user.Id);

        logger.LogInformation("Usuario marcado como conectado");

        logger.LogInformation("Inicio de sesión completado correctamente para el usuario: {UserId}", user.Id);

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }
}
