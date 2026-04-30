namespace uni_chat_backend.Infrastructure.Configuration;

public class RefreshTokenSettings
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public int ExpireDays { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
}

