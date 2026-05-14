using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Login.Interfaces;

namespace uni_chat_backend.Features.Auth.Login.Cache;

public class UserLoginSessionCache(
    IConnectionMultiplexer redis
) : IUserLoginSessionCache
{
    private readonly IDatabase _db =
        redis.GetDatabase();

    public async Task CreateSessionAsync(User user)
    {
        var sessionData = new
        {
            user.Id,
            user.Username,
            user.Phone,
            LoggedAt = DateTime.UtcNow
        };

        await _db.StringSetAsync(
            $"session:{user.Id}",
            JsonSerializer.Serialize(sessionData),
            TimeSpan.FromHours(1)
        );
    }

    public async Task MarkOnlineAsync(Guid userId)
    {
        await _db.StringSetAsync(
            $"user:{userId}:online",
            "true",
            TimeSpan.FromMinutes(30)
        );
    }
}