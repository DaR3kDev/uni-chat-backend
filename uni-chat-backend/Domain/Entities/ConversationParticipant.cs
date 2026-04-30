using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Domain.Entities;

public class ConversationParticipant
{
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public ParticipantRole Role { get; set; } = ParticipantRole.MEMBER;

    public bool CanWrite { get; set; } = true;
    public bool CanRead { get; set; } = true;
    public bool IsBanned { get; set; } = false;
}