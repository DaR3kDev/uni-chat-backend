using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.SendMessage;

public record SendMessageResult
(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Content,
    string? FileUrl,
    string? FileName,
    MessageType Type,
    DateTime CreatedAt
);