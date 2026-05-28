using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.SendMessage.Contracts;

public sealed record MessageSent(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Content,
    string? FileUrl,
    string? FileName,
    MessageType Type,
    DateTime CreatedAt
);

