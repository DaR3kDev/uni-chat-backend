using StackExchange.Redis;
using uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Cache;

public class JoinConversationCache(IConnectionMultiplexer redis) : IJoinConversationCache
{
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(30);

    public async Task SetUserOnlineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringSetAsync(JoinConversationCacheKeys.UserOnline(userId), "true", _expiration);
    }

    public async Task SetActiveConversationAsync(Guid userId, Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringSetAsync(JoinConversationCacheKeys.ActiveConversation(userId), conversationId.ToString(),
            _expiration);
    }
}
