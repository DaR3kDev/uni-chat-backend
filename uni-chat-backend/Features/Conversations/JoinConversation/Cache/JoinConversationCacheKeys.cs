namespace uni_chat_backend.Features.Conversations.JoinConversation.Cache;

public static class JoinConversationCacheKeys
{
    public static string UserOnline(Guid userId) => $"user:{userId}:online";

    public static string ActiveConversation(Guid userId) => $"user:{userId}:active-conversation";
}

