namespace uni_chat_backend.Features.Conversations.JoinConversation;

public record JoinConversationResult
(
     Guid ConversationId,
     bool IsMember
);

