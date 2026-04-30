using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.MarkAsRead;

public class MarkAsReadHandler(
    IMessageRepository messageRepository,
    ICurrentUserService currentUser
) : IRequestHandler<MarkAsReadCommand, MarkAsReadResult>
{   
    public async Task<MarkAsReadResult> Handle(
     MarkAsReadCommand request,
     CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autenticado");

        var updatedCount = await messageRepository
            .MarkConversationAsReadAsync(request.ConversationId, userId);

        return new MarkAsReadResult(
            request.ConversationId,
            updatedCount
        );
    }
}