namespace uni_chat_backend.Features.Conversations.Shared.Contracts;

public sealed record ConversationsViewed(
    Guid UserId,
    DateTime ViewedAt
);
