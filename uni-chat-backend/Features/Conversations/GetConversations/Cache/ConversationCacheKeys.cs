namespace uni_chat_backend.Features.Conversations.GetConversations.Cache;

public static class ConversationCacheKeys
{
    public static string Conversations(Guid userId) => $"conversations:{userId}";

    public static string UserOnline(Guid userId) => $"user:{userId}:online";
}
