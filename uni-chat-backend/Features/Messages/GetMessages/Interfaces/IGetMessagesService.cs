using uni_chat_backend.Features.Messages.GetMessages.Contracts;

namespace uni_chat_backend.Features.Messages.GetMessages.Interfaces;

public interface IGetMessagesService
{
    Task<List<GetMessagesResult>> ExecuteAsync(GetMessagesQuery request, CancellationToken cancellationToken);
}

