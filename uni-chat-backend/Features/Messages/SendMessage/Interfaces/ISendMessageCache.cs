namespace uni_chat_backend.Features.Messages.SendMessage.Interfaces;

public interface ISendMessageCache
{
    Task RemoveMessagesAsync(Guid conversationId, CancellationToken cancellationToken);

    Task RemoveConversationsAsync(Guid userId, CancellationToken cancellationToken);

    Task IncrementUnreadAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken);
}

