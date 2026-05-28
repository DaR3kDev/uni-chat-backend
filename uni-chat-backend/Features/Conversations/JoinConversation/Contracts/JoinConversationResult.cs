namespace uni_chat_backend.Features.Conversations.JoinConversation.Contracts;

public record JoinConversationResult(
    Guid ConversationId,
    bool IsMember
);
