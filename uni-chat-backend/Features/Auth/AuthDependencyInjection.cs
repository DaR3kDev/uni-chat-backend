using uni_chat_backend.Features.Auth.Login;
using uni_chat_backend.Features.Auth.Logout;
using uni_chat_backend.Features.Auth.Me;
using uni_chat_backend.Features.Auth.Refresh;
using uni_chat_backend.Features.Auth.Register;

namespace uni_chat_backend.Features.Auth;

public static class AuthDependencyInjection
{
    public static void AddAuthFeatures(this IServiceCollection services)
    {
        services.AddRegisterFeature();
        services.AddLoginFeature();
        services.AddLogoutFeature();
        services.AddMeFeature();
        services.AddRefreshFeature();
    }
}