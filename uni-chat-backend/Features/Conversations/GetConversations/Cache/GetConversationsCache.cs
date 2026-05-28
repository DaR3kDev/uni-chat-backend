using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Features.Conversations.GetConversations.Contracts;
using uni_chat_backend.Features.Conversations.GetConversations.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations.Cache;

public class GetConversationsCache(IConnectionMultiplexer redis) : IGetConversationsCache
{
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<List<GetConversationsResult>?> GetAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        var cached = await db.StringGetAsync(ConversationCacheKeys.Conversations(userId));

        if (!cached.HasValue) return null;

        try
        {
            string json = cached!;

            return JsonSerializer.Deserialize<List<GetConversationsResult>>(json, Options);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(Guid userId, List<GetConversationsResult> response,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringSetAsync(ConversationCacheKeys.Conversations(userId),
            JsonSerializer.Serialize(response, Options), _expiration);
    }

    public async Task RemoveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync(ConversationCacheKeys.Conversations(userId));
    }

    public async Task<Dictionary<Guid, bool>> GetOnlineStatusesAsync(List<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        var tasks = userIds.ToDictionary(id => id, id => db.StringGetAsync(ConversationCacheKeys.UserOnline(id)));

        await Task.WhenAll(tasks.Values);

        return tasks.ToDictionary(x => x.Key, x => x.Value.Result == "true");
    }
}
