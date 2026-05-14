namespace uni_chat_backend.Features.Auth.Refresh.Interfaces;

public interface IRefreshCookieService
{
    void SetRefreshToken(HttpContext context, string refreshToken);
}