namespace uni_chat_backend.Features.Conversations.GetConversations;

public record GetConversationsResult
(
    Guid ConversationId,
    Guid ContactUserId,
    string? Username,
    bool IsOnline,
    DateTime? LastSeen,
    DateTime CreatedAt,
    DateTime? LastMessageAt
);

