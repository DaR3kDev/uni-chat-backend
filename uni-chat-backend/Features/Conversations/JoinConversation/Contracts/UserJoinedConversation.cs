namespace uni_chat_backend.Features.Conversations.JoinConversation.Contracts;

public sealed record UserJoinedConversation(
    Guid ConversationId,
    Guid UserId,
    DateTime JoinedAt
);

