using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Register.Interfaces;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Register.Services;

public class RegisterService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService,
    IUserRegistrationValidator validator,
    IUserSessionCache sessionCache,
    IHttpContextAccessor httpContextAccessor,
    ILogger<RegisterService> logger)
{
    public async Task<AuthResponse> RegisterAsync(RegisterCommand request, CancellationToken ct)
    {
        var requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "desconocido";

        logger.LogInformation("[{RequestId}] Iniciando proceso de registro de usuario", requestId);

        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim().ToLowerInvariant();

        logger.LogInformation("[{RequestId}] Validando disponibilidad del correo y nombre de usuario", requestId);

        await validator.ValidateAsync(email, username);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            Phone = request.Phone,
            IsOnline = true,
            CreatedAt = DateTime.UtcNow
        };

        logger.LogInformation("[{RequestId}] Creando usuario con ID: {UserId}", requestId, user.Id);

        await userRepository.CreateAsync(user);

        logger.LogInformation("[{RequestId}] Generando tokens de autenticación", requestId);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.CreateAsync(refreshToken);

        logger.LogInformation("[{RequestId}] Creando sesión del usuario y marcando estado en línea", requestId);

        await sessionCache.CreateSessionAsync(user);
        await sessionCache.MarkOnlineAsync(user.Id);

        logger.LogInformation("[{RequestId}] Registro completado correctamente para el usuario: {UserId}", requestId,
            user.Id);

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }
}