using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Features.Messages.SendMessage.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Messages.SendMessage.Consumers;

public class MessageSentCacheConsumer(
    ISendMessageCache cache,
    IConversationRepository conversationRepository,
    ILogger<MessageSentCacheConsumer> logger)
{
    public async Task Handle(MessageSent message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Invalidando caché de conversación. ConversationId: {ConversationId}",
            message.ConversationId);

        await cache.RemoveMessagesAsync(message.ConversationId, cancellationToken);

        var conversation = await conversationRepository.GetByIdAsync(message.ConversationId);

        if (conversation is null) return;

        await Task.WhenAll(
            conversation.Participants.Select(p => cache.RemoveConversationsAsync(p.UserId, cancellationToken)));

        await Task.WhenAll(conversation.Participants.Where(p => p.UserId != message.SenderId)
            .Select(p => cache.IncrementUnreadAsync(message.ConversationId, p.UserId, cancellationToken)));
    }
}
