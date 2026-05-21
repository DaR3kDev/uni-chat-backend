using uni_chat_backend.Features.Auth.Refresh.Interfaces;

namespace uni_chat_backend.Features.Auth.Refresh.Cookies;

public class RefreshCookieService : IRefreshCookieService
{
    public void SetRefreshToken(HttpContext context, string refreshToken)
    {
        context.Response.Cookies.Append("refreshToken", refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
    }
}
