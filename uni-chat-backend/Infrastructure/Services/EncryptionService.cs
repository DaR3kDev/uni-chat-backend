namespace uni_chat_backend.Infrastructure.Services;

public class EncryptionService
{
    public string Encrypt(string text)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
    }

    public string Decrypt(string cipher)
    {
        return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cipher));
    }
}

