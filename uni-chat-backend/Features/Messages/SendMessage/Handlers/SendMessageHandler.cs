using MediatR;
using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Features.Messages.SendMessage.Interfaces;

namespace uni_chat_backend.Features.Messages.SendMessage.Handlers;

public class SendMessageHandler(ISendMessageService service) : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        return await service.ExecuteAsync(request, cancellationToken);
    }
}
