using StackExchange.Redis;
using uni_chat_backend.Features.Auth.Logout.Interfaces;

namespace uni_chat_backend.Features.Auth.Logout.Cache;

public class LogoutSessionCache(IConnectionMultiplexer redis) : ILogoutSessionCache
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task RemoveSessionAsync(string userId)
    {
        await _db.KeyDeleteAsync($"session:{userId}");
    }

    public async Task SetOfflineAsync(string userId)
    {
        await _db.StringSetAsync($"user:{userId}:online", "false");
    }
}