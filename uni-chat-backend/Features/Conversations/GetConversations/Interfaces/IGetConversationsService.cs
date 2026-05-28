using uni_chat_backend.Features.Conversations.GetConversations.Contracts;

namespace uni_chat_backend.Features.Conversations.GetConversations.Interfaces;

public interface IGetConversationsService
{
    Task<List<GetConversationsResult>> ExecuteAsync(CancellationToken cancellationToken);
}
