namespace uni_chat_backend.Features.Messages.SendMessage;

public record SendMessageResult
(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime CreatedAt,
    bool Delivered
);