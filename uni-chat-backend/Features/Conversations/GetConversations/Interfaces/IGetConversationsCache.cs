using uni_chat_backend.Features.Conversations.GetConversations.Contracts;

namespace uni_chat_backend.Features.Conversations.GetConversations.Interfaces;

public interface IGetConversationsCache
{
    Task<List<GetConversationsResult>?> GetAsync(Guid userId, CancellationToken cancellationToken);

    Task SetAsync(Guid userId, List<GetConversationsResult> response, CancellationToken cancellationToken);

    Task RemoveAsync(Guid userId, CancellationToken cancellationToken);

    Task<Dictionary<Guid, bool>> GetOnlineStatusesAsync(List<Guid> userIds, CancellationToken cancellationToken);
}
