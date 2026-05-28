using StackExchange.Redis;
using uni_chat_backend.Features.Messages.SendMessage.Interfaces;

namespace uni_chat_backend.Features.Messages.SendMessage.Cache;

public class SendMessageCache(IConnectionMultiplexer redis) : ISendMessageCache
{
    public async Task RemoveMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync(SendMessageCacheKeys.Messages(conversationId));
    }

    public async Task RemoveConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.KeyDeleteAsync(SendMessageCacheKeys.Conversations(userId));
    }

    public async Task IncrementUnreadAsync(Guid conversationId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        await db.StringIncrementAsync(SendMessageCacheKeys.Unread(conversationId, userId));
    }
}
