using MediatR;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Handlers;

public class GetOrCreateConversationHandler(IGetOrCreateConversationService service)
    : IRequestHandler<GetOrCreateConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(GetOrCreateConversationCommand request,
        CancellationToken cancellationToken)
    {
        return await service.ExecuteAsync(request, cancellationToken);
    }
}

