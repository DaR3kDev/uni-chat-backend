namespace uni_chat_backend.Features.Conversations.Shared.DTOs;

public sealed record ConversationDto(
    Guid Id,
    DateTime CreatedAt,
    DateTime? LastMessageAt
);
