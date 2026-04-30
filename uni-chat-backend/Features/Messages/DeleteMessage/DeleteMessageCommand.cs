using MediatR;

namespace uni_chat_backend.Features.Messages.DeleteMessage;

public record DeleteMessageCommand(
    Guid MessageId
) : IRequest;