using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.SendMessage;

public sealed record SendMessageEvent(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string? Content,
    MessageType Type,
    DateTime CreatedAt
);
