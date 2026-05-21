using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Infrastructure.SignalR;

namespace uni_chat_backend.Features.Messages.SendMessage;

public sealed class SendMessageConsumer(IHubContext<ChatHub> hub, ILogger<SendMessageConsumer> logger)
{
    private readonly IHubContext<ChatHub> _hub = hub;
    private readonly ILogger<SendMessageConsumer> _logger = logger;

    public async Task Handle(SendMessageEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enviando mensaje realtime {MessageId}", message.MessageId);

        await _hub.Clients
            .Group(message.ConversationId.ToString())
            .SendAsync(
                "ReceiveMessage",
                message,
                cancellationToken
            );
    }
}
