namespace uni_chat_backend.Domain.Entities;

public class EncryptionData
{
    public string? CipherText { get; set; } 
    public string? Nonce { get; set; } 
    public string? PublicKey { get; set; }
    public string? Algorithm { get; set; }
    public DateTime EncryptedAt { get; set; } = DateTime.UtcNow;
}

