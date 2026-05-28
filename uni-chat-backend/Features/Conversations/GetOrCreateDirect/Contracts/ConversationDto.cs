namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;

public record ConversationDto(
    Guid Id,
    Guid ContactUserId,
    DateTime CreatedAt,
    DateTime? LastMessageAt
);
