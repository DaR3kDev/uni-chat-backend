namespace uni_chat_backend.Features.Auth.Logout.Interfaces;

public interface ILogoutSessionCache
{
    Task RemoveSessionAsync(string userId);

    Task SetOfflineAsync(string userId);
}