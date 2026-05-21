using uni_chat_backend.Features.Auth.Login.Cache;
using uni_chat_backend.Features.Auth.Login.Interfaces;
using uni_chat_backend.Features.Auth.Login.Services;
using uni_chat_backend.Features.Auth.Login.Validators;

namespace uni_chat_backend.Features.Auth.Login;

public static class LoginDependencyInjection
{
    public static void AddLoginFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<LoginService>();

        // =========================================================
        // VALIDATORS
        // =========================================================
        services.AddScoped<IUserLoginValidator, UserLoginValidator>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IUserLoginSessionCache, UserLoginSessionCache>();
    }
}
