using uni_chat_backend.Features.Auth.Register.Cache;
using uni_chat_backend.Features.Auth.Register.Interfaces;
using uni_chat_backend.Features.Auth.Register.Services;
using uni_chat_backend.Features.Auth.Register.Validators;

namespace uni_chat_backend.Features.Auth.Register;

public static class RegisterDependencyInjection
{
    public static void AddRegisterFeature(this IServiceCollection services)
    {
        services.AddScoped<RegisterService>();

        // =========================================================
        // VALIDATORS
        // =========================================================
        services.AddScoped<IUserRegistrationValidator, UserRegistrationValidator>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IUserSessionCache, UserSessionCache>();
    }
}