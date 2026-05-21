using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Me.Interfaces;

namespace uni_chat_backend.Features.Auth.Me.Cache;

public class MeUserCache(IConnectionMultiplexer redis) : IMeUserCache
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<User?> GetAsync(Guid userId)
    {
        var cacheKey = $"user:{userId}";

        var cachedUser = await _db.StringGetAsync(cacheKey);

        if (!cachedUser.HasValue) return null;

        var cachedJson = cachedUser.ToString();

        if (string.IsNullOrWhiteSpace(cachedJson)) return null;

        return JsonSerializer.Deserialize<User>(cachedJson);
    }

    public async Task SetAsync(User user)
    {
        var cacheKey = $"user:{user.Id}";

        var serializedUser = JsonSerializer.Serialize(user);

        await _db.StringSetAsync(cacheKey, serializedUser, TimeSpan.FromMinutes(10));
    }
}
