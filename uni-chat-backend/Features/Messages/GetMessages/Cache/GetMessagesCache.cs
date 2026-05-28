using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Features.Messages.GetMessages.Contracts;
using uni_chat_backend.Features.Messages.GetMessages.Interfaces;

namespace uni_chat_backend.Features.Messages.GetMessages.Cache;

public class GetMessagesCache(IConnectionMultiplexer redis) : IGetMessagesCache
{
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task<List<GetMessagesResult>?> GetAsync(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        var cached = await db.StringGetAsync(GetMessagesCacheKeys.Messages(conversationId));

        if (!cached.HasValue) return null;

        try
        {
            string json = cached!;

            return JsonSerializer.Deserialize<List<GetMessagesResult>>(json, Options);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(Guid conversationId, List<GetMessagesResult> messages,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringSetAsync(GetMessagesCacheKeys.Messages(conversationId),
            JsonSerializer.Serialize(messages, Options), _expiration);
    }

    public async Task RemoveAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync(GetMessagesCacheKeys.Messages(conversationId));
    }
}

