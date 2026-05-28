using MediatR;
using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;
using uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Handlers;

public class JoinConversationHandler(IJoinConversationService service)
    : IRequestHandler<JoinConversationCommand, JoinConversationResult>
{
    public async Task<JoinConversationResult> Handle(JoinConversationCommand request,
        CancellationToken cancellationToken)
    {
        return await service.ExecuteAsync(request, cancellationToken);
    }
}
