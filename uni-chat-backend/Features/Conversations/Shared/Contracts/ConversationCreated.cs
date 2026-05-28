namespace uni_chat_backend.Features.Conversations.Shared.Contracts;

public sealed record ConversationCreated(
    Guid ConversationId,
    Guid CreatedBy,
    DateTime CreatedAt
);
