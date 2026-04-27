using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;
using uni_chat_backend.Infrastructure.Configuration;

namespace uni_chat_backend.Infrastructure.Security;

public class Argon2PasswordHasher(Argon2Settings settings)
{
    private readonly Argon2Settings _settings = settings;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_settings.SaltSize);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = _settings.Iterations,
            MemorySize = _settings.MemorySize,
            DegreeOfParallelism = _settings.DegreeOfParallelism
        };

        var hash = argon2.GetBytes(_settings.HashSize);

        return string.Join(".",
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash),
            _settings.Iterations,
            _settings.MemorySize,
            _settings.DegreeOfParallelism
        );
    }

    public bool Verify(string password, string stored)
    {
        var parts = stored.Split('.');

        if (parts.Length != 5)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        var iterations = int.Parse(parts[2]);
        var memory = int.Parse(parts[3]);
        var parallelism = int.Parse(parts[4]);

        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = iterations,
            MemorySize = memory,
            DegreeOfParallelism = parallelism
        };

        var newHash = argon2.GetBytes(hash.Length);

        return CryptographicOperations.FixedTimeEquals(newHash, hash);
    }
}
