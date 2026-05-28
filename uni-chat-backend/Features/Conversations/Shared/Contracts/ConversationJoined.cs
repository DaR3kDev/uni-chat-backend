namespace uni_chat_backend.Features.Conversations.Shared.Contracts;

public sealed record ConversationJoined(
    Guid ConversationId,
    Guid UserId,
    DateTime JoinedAt
);

