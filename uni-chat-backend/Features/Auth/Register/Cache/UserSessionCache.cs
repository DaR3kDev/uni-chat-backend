using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Register.Interfaces;

namespace uni_chat_backend.Features.Auth.Register.Cache;

public class UserSessionCache(IConnectionMultiplexer redis) : IUserSessionCache
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task CreateSessionAsync(User user)
    {
        var session = new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Phone,
            LoggedAt = DateTime.UtcNow
        };

        await _db.StringSetAsync($"session:{user.Id}", JsonSerializer.Serialize(session), TimeSpan.FromHours(1));
    }

    public async Task MarkOnlineAsync(Guid userId)
    {
        await _db.StringSetAsync($"user:{userId}:online", "true", TimeSpan.FromMinutes(30));
    }
}
