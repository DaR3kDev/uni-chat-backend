namespace uni_chat_backend.Features.Conversations.GetConversations;

public record GetConversationsResult
(
    Guid Id,
    Guid ContactUserId,
    DateTime CreatedAt,
    DateTime? LastMessageAt
);

