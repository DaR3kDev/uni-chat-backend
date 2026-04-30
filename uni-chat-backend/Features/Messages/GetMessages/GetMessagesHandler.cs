using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.GetMessages;

public class GetMessagesHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetMessagesQuery, List<GetMessagesResult>>
{
    public async Task<List<GetMessagesResult>> Handle(
        GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autenticado");

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId)
            ?? throw new InvalidOperationException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("No perteneces a esta conversación");

        var messages = await messageRepository.GetByConversationIdAsync(request.ConversationId);

        var key = await conversationRepository.GetEncryptionKeyAsync(request.ConversationId);
        var aesKey = Convert.FromBase64String(key);

        return [.. messages.Select(m => new GetMessagesResult(
            m.Id,
            m.ConversationId,
            m.SenderId,
            E2EEncryptionService.Decrypt(m.Content!, aesKey),
            m.CreatedAt
        ))];
    }
}