namespace uni_chat_backend.Features.Conversations.Shared.DTOs;

public sealed record ParticipantDto(
    Guid UserId,
    string Username,
    bool IsOnline
);
