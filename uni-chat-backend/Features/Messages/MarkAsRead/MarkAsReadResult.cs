namespace uni_chat_backend.Features.Messages.MarkAsRead;

public record MarkAsReadResult
(
    Guid ConversationId,
    int MessagesUpdated
);

