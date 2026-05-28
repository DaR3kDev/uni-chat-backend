using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;

public interface IJoinConversationService
{
    Task<JoinConversationResult> ExecuteAsync(JoinConversationCommand request, CancellationToken cancellationToken);
}

