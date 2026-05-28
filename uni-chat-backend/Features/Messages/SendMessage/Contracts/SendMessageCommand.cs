using MediatR;
using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Features.Messages.SendMessage.Contracts;

public record SendMessageCommand(
    Guid ConversationId,
    string? Content,
    string? FileUrl,
    string? FileName,
    MessageType Type
) : IRequest<SendMessageResult>;
