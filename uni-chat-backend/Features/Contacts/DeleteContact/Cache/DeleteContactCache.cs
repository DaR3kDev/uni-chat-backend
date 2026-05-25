using StackExchange.Redis;
using uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;

namespace uni_chat_backend.Features.Contacts.DeleteContact.Cache;

public class DeleteContactCache(IConnectionMultiplexer redis) : IDeleteContactCache
{
    public async Task IncrementContactsVersionAsync(Guid userId)
    {
        var db = redis.GetDatabase();

        await db.StringIncrementAsync($"contacts:{userId}:version");
    }
}
