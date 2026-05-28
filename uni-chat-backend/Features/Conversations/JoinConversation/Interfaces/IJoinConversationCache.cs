namespace uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;

public interface IJoinConversationCache
{
    Task SetUserOnlineAsync(Guid userId, CancellationToken cancellationToken);

    Task SetActiveConversationAsync(Guid userId, Guid conversationId, CancellationToken cancellationToken);
}

