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
    ILogger<RegisterService> logger)
{
    public async Task<AuthResponse> RegisterAsync(RegisterCommand request, CancellationToken ct)
    {
        logger.LogInformation("Iniciando proceso de registro de usuario");

        var email = request.Email.Trim().ToLowerInvariant();
        var username = request.Username.Trim().ToLowerInvariant();

        logger.LogInformation("Validando disponibilidad del correo y nombre de usuario");

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

        logger.LogInformation("Creando usuario con ID: {UserId}", user.Id);

        await userRepository.CreateAsync(user);

        logger.LogInformation("Generando tokens de autenticación");

        var accessToken = tokenService.GenerateAccessToken(user);

        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.CreateAsync(refreshToken);

        logger.LogInformation("Creando sesión del usuario y marcando estado en línea");

        await sessionCache.CreateSessionAsync(user);

        await sessionCache.MarkOnlineAsync(user.Id);

        logger.LogInformation("Registro completado correctamente para el usuario: {UserId}", user.Id);

        return new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken.Token };
    }
}
