using uni_chat_backend.Features.Auth.Logout.Cache;
using uni_chat_backend.Features.Auth.Logout.Interfaces;
using uni_chat_backend.Features.Auth.Logout.Services;
using uni_chat_backend.Features.Auth.Logout.Tokens;

namespace uni_chat_backend.Features.Auth.Logout;

public static class LogoutDependencyInjection
{
    public static void AddLogoutFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<LogoutService>();

        // =========================================================
        // TOKENS
        // =========================================================
        services.AddScoped<IRefreshTokenRevoker, RefreshTokenRevoker>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<ILogoutSessionCache, LogoutSessionCache>();
    }
}