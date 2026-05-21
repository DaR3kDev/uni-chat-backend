using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.GetMessages;

public record GetMessagesResult(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Content,
    string? FileUrl,
    string? FileName,
    MessageType Type,
    DateTime CreatedAt
);
