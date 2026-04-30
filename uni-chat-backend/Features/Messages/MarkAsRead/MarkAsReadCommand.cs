using MediatR;

namespace uni_chat_backend.Features.Messages.MarkAsRead;

public record MarkAsReadCommand(
    Guid ConversationId
) : IRequest<MarkAsReadResult>;