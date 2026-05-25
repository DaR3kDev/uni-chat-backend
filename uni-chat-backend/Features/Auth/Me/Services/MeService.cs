using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Me.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Auth.Me.Services;

public class MeService(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IMeUserCache userCache,
    ILogger<MeService> logger)
{
    public async Task<User> GetCurrentUserAsync()
    {
        logger.LogInformation("Iniciando obtención del usuario autenticado");

        var userId = currentUserService.UserId ?? throw new UnauthorizedException("Usuario no autenticado");

        logger.LogInformation("Buscando usuario en caché: {UserId}", userId);

        var cachedUser = await userCache.GetAsync(userId);

        if (cachedUser is not null)
        {
            logger.LogInformation("Usuario obtenido desde caché: {UserId}", userId);

            return cachedUser;
        }

        logger.LogInformation("Usuario no encontrado en caché, consultando base de datos: {UserId}", userId);

        var user = await userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("Usuario no encontrado");

        logger.LogInformation("Guardando usuario en caché: {UserId}", userId);

        await userCache.SetAsync(user);

        logger.LogInformation("Usuario obtenido correctamente: {UserId}", userId);

        return user;
    }
}
