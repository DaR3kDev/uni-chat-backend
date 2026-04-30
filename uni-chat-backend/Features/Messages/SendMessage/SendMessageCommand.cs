using MediatR;

namespace uni_chat_backend.Features.Messages.SendMessage;

public record SendMessageCommand
(
    Guid ConversationId,
    string Content
) : IRequest<SendMessageResult>;