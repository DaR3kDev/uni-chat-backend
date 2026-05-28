using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;

public interface IGetOrCreateConversationCache
{
    Task<ConversationDto?> GetAsync(Guid ownerUserId, Guid contactUserId, CancellationToken cancellationToken);

    Task SetAsync(Guid ownerUserId, Guid contactUserId, ConversationDto conversation,
        CancellationToken cancellationToken);

    Task RemoveConversationsAsync(Guid userId, CancellationToken cancellationToken);
}

