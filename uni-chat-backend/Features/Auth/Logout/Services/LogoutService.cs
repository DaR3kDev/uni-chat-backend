using uni_chat_backend.Features.Auth.Logout.Interfaces;

namespace uni_chat_backend.Features.Auth.Logout.Services;

public class LogoutService(
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenRevoker refreshTokenRevoker,
    ILogoutSessionCache sessionCache,
    ILogger<LogoutService> logger)
{
    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;

        ArgumentNullException.ThrowIfNull(httpContext);

        logger.LogInformation("Iniciando proceso de cierre de sesión");

        var refreshToken = httpContext.Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogInformation("Revocando refresh token del usuario");

            await refreshTokenRevoker.RevokeAsync(refreshToken);
        }

        var userId = httpContext.User.FindFirst("sub")?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            logger.LogInformation("Eliminando sesión del usuario: {UserId}", userId);

            await sessionCache.RemoveSessionAsync(userId);

            await sessionCache.SetOfflineAsync(userId);

            logger.LogInformation("Usuario marcado como desconectado: {UserId}", userId);
        }

        httpContext.Response.Cookies.Delete("refreshToken");

        logger.LogInformation("Cookie de refresh token eliminada correctamente");

        logger.LogInformation("Cierre de sesión completado correctamente");
    }
}
