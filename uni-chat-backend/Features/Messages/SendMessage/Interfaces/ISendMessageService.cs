using uni_chat_backend.Features.Messages.SendMessage.Contracts;

namespace uni_chat_backend.Features.Messages.SendMessage.Interfaces;

public interface ISendMessageService
{
    Task<SendMessageResult> ExecuteAsync(SendMessageCommand request, CancellationToken cancellationToken);
}

