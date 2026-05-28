using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;

public interface IGetOrCreateConversationService
{
    Task<ConversationDto> ExecuteAsync(GetOrCreateConversationCommand request, CancellationToken cancellationToken);
}

