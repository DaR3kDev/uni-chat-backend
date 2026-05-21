using uni_chat_backend.Features.Auth.Refresh.Cache;
using uni_chat_backend.Features.Auth.Refresh.Cookies;
using uni_chat_backend.Features.Auth.Refresh.Interfaces;
using uni_chat_backend.Features.Auth.Refresh.Services;
using uni_chat_backend.Features.Auth.Refresh.Validators;

namespace uni_chat_backend.Features.Auth.Refresh;

public static class RefreshDependencyInjection
{
    public static void AddRefreshFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<RefreshService>();

        // =========================================================
        // VALIDATORS
        // =========================================================
        services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IRefreshSessionCache, RefreshSessionCache>();

        // =========================================================
        // COOKIES
        // =========================================================
        services.AddScoped<IRefreshCookieService, RefreshCookieService>();
    }
}
