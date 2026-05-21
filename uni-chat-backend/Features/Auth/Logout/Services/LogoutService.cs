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

        var requestId = httpContext.Items["RequestId"]?.ToString() ?? "desconocido";

        logger.LogInformation("[{RequestId}] Iniciando proceso de cierre de sesión", requestId);

        var refreshToken = httpContext.Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            logger.LogInformation("[{RequestId}] Revocando refresh token del usuario", requestId);

            await refreshTokenRevoker.RevokeAsync(refreshToken);
        }

        var userId = httpContext.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            logger.LogInformation("[{RequestId}] Eliminando sesión del usuario: {UserId}", requestId, userId);

            await sessionCache.RemoveSessionAsync(userId);

            await sessionCache.SetOfflineAsync(userId);

            logger.LogInformation("[{RequestId}] Usuario marcado como desconectado: {UserId}", requestId, userId);
        }

        httpContext.Response.Cookies.Delete("refreshToken");

        logger.LogInformation("[{RequestId}] Cookie de refresh token eliminada correctamente", requestId);

        logger.LogInformation("[{RequestId}] Cierre de sesión completado correctamente", requestId);
    }
}
