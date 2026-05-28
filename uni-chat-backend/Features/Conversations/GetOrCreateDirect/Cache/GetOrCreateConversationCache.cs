using StackExchange.Redis;
using System.Text.Json;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Cache;

public class GetOrCreateConversationCache(IConnectionMultiplexer redis) : IGetOrCreateConversationCache
{
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<ConversationDto?> GetAsync(Guid ownerUserId, Guid contactUserId,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        var cached = await db.StringGetAsync(
            DirectConversationCacheKeys.DirectConversation(ownerUserId, contactUserId));

        if (!cached.HasValue) return null;

        try
        {
            string json = cached!;

            return JsonSerializer.Deserialize<ConversationDto>(json, Options);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(Guid ownerUserId, Guid contactUserId, ConversationDto conversation,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringSetAsync(DirectConversationCacheKeys.DirectConversation(ownerUserId, contactUserId),
            JsonSerializer.Serialize(conversation, Options), _expiration);
    }

    public async Task RemoveConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync(DirectConversationCacheKeys.Conversations(userId));
    }
}

