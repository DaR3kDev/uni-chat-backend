using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Infrastructure.SignalR;

namespace uni_chat_backend.Features.Messages.SendMessage.Consumers;

public class MessageSentRealtimeConsumer(IHubContext<ChatHub> hub, ILogger<MessageSentRealtimeConsumer> logger)
{
    public async Task Handle(MessageSent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Emitiendo mensaje en tiempo real. MessageId: {MessageId}", message.MessageId);

        await hub.Clients.Group(message.ConversationId.ToString())
            .SendAsync("ReceiveMessage",
                new
                {
                    id = message.MessageId,
                    conversationId = message.ConversationId,
                    senderId = message.SenderId,
                    content = message.Content,
                    fileUrl = message.FileUrl,
                    fileName = message.FileName,
                    createdAt = message.CreatedAt,
                    type = message.Type.ToString().ToUpper(),
                    status = "sent"
                }, cancellationToken);

        logger.LogInformation("Mensaje en tiempo real enviado correctamente. MessageId: {MessageId}",
            message.MessageId);
    }
}
