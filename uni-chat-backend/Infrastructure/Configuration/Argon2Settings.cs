namespace uni_chat_backend.Infrastructure.Configuration;

public class Argon2Settings
{
    public int Iterations { get; set; } = 3;
    public int MemorySize { get; set; } = 131072;
    public int DegreeOfParallelism { get; set; } = 2;
    public int SaltSize { get; set; } = 16;
    public int HashSize { get; set; } = 32;
}

