using uni_chat_backend.Features.Auth.Me.Cache;
using uni_chat_backend.Features.Auth.Me.Interfaces;
using uni_chat_backend.Features.Auth.Me.Services;

namespace uni_chat_backend.Features.Auth.Me;

public static class MeDependencyInjection
{
    public static void AddMeFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<MeService>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IMeUserCache, MeUserCache>();
    }
}
