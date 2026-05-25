using StackExchange.Redis;
using uni_chat_backend.Features.Contacts.AddContact.Interfaces;

namespace uni_chat_backend.Features.Contacts.AddContact.Cache;

public class ContactCache(IConnectionMultiplexer redis) : IContactCache
{
    public async Task IncrementContactsVersionAsync(Guid userId)
    {
        var db = redis.GetDatabase();

        await db.StringIncrementAsync($"contacts:{userId}:version");
    }
}
