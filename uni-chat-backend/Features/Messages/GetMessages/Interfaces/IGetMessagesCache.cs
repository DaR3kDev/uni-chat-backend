using uni_chat_backend.Features.Messages.GetMessages.Contracts;

namespace uni_chat_backend.Features.Messages.GetMessages.Interfaces;

public interface IGetMessagesCache
{
    Task<List<GetMessagesResult>?> GetAsync(Guid conversationId, CancellationToken cancellationToken);

    Task SetAsync(Guid conversationId, List<GetMessagesResult> messages, CancellationToken cancellationToken);

    Task RemoveAsync(Guid conversationId, CancellationToken cancellationToken);
}

