namespace uni_chat_backend.Features.Conversations.Shared.Services;

public class ConversationAuthorizationService
{
    public bool CanAccess(Guid currentUserId, Guid participantId)
    {
        return currentUserId != participantId;
    }
}
