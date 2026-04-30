using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.JoinConversation;

public class JoinConversationHandler(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser
) : IRequestHandler<JoinConversationCommand, JoinConversationResult>
{
    public async Task<JoinConversationResult> Handle(
        JoinConversationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autorizado");

        var isMember = await conversationRepository.IsUserInConversationAsync(
            request.ConversationId,
            userId
        );

        if (!isMember)
            throw new UnauthorizedAccessException("No tienes acceso a esta conversación");

        await conversationRepository.SetUserOnlineAsync(userId);

        return new JoinConversationResult(
            request.ConversationId,
            true
        );
    }
}