namespace uni_chat_backend.Features.Messages.GetMessages;

public record GetMessagesResult(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime CreatedAt
);

