namespace uni_chat_backend.Features.Auth.Logout.Interfaces;

public interface IRefreshTokenRevoker
{
    Task RevokeAsync(string refreshToken);
}