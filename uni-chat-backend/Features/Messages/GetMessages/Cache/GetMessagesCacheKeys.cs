namespace uni_chat_backend.Features.Messages.GetMessages.Cache;

public static class GetMessagesCacheKeys
{
    public static string Messages(Guid conversationId) => $"messages:{conversationId}";
}

