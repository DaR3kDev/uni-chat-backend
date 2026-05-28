namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Cache;

public static class DirectConversationCacheKeys
{
    public static string DirectConversation(Guid ownerUserId, Guid contactUserId) =>
        $"conversation:direct:{ownerUserId}:{contactUserId}";

    public static string Conversations(Guid userId) => $"conversations:{userId}";
}

