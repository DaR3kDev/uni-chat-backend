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
    ILogger<MeService> logger,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var requestId = httpContextAccessor.HttpContext?.Items["RequestId"]?.ToString() ?? "desconocido";

        logger.LogInformation("[{RequestId}] Iniciando obtención del usuario autenticado", requestId);

        var userId = currentUserService.UserId ?? throw new UnauthorizedException("Usuario no autenticado");

        logger.LogInformation("[{RequestId}] Buscando usuario en caché: {UserId}", requestId, userId);

        var cachedUser = await userCache.GetAsync(userId);

        if (cachedUser is not null)
        {
            logger.LogInformation("[{RequestId}] Usuario obtenido desde caché: {UserId}", requestId, userId);

            return cachedUser;
        }

        logger.LogInformation("[{RequestId}] Usuario no encontrado en caché, consultando base de datos: {UserId}",
            requestId, userId);

        var user = await userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("Usuario no encontrado");

        logger.LogInformation("[{RequestId}] Guardando usuario en caché: {UserId}", requestId, userId);

        await userCache.SetAsync(user);

        logger.LogInformation("[{RequestId}] Usuario obtenido correctamente: {UserId}", requestId, userId);

        return user;
    }
}
