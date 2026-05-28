namespace uni_chat_backend.Features.Messages.SendMessage.Cache;

public static class SendMessageCacheKeys
{
    public static string Messages(Guid conversationId) => $"messages:{conversationId}";

    public static string Conversations(Guid userId) => $"conversations:{userId}";

    public static string Unread(Guid conversationId, Guid userId) => $"conversation:{conversationId}:unread:{userId}";
}
