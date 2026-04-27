namespace uni_chat_backend.Features.Auth.Shared;

public class AuthResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}

