using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Features.Contacts.GetContacts.Interfaces;
using uni_chat_backend.Features.Contacts.Shared;

namespace uni_chat_backend.Features.Contacts.GetContacts.Cache;

public class GetContactsCache(IConnectionMultiplexer redis) : IGetContactsCache
{
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<List<ContactResponse>?> GetAsync(Guid ownerUserId, GetContactsQuery query)
    {
        var db = redis.GetDatabase();

        var key = BuildKey(ownerUserId, query);

        var cached = await db.StringGetAsync(key);

        if (!cached.HasValue)
            return null;

        try
        {
            string json = cached!;
            return JsonSerializer.Deserialize<List<ContactResponse>>(json, Options);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetAsync(Guid ownerUserId, GetContactsQuery query, List<ContactResponse> response)
    {
        var db = redis.GetDatabase();

        var key = BuildKey(ownerUserId, query);

        await db.StringSetAsync(
            key,
            JsonSerializer.Serialize(response, Options),
            _expiration
        );
    }

    private static string BuildKey(Guid userId, GetContactsQuery query)
        => $"contacts:{userId}:{query.Page}:{query.PageSize}:{query.Search}";
}
