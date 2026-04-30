using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect;

public class GetOrCreateConversationHandler(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetOrCreateConversationCommand, ConversationDto>
{
    public async Task<ConversationDto> Handle(
        GetOrCreateConversationCommand request,
        CancellationToken ct)
    {
        var ownerUserId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("Usuario no autenticado");

        var existingConversation = await conversationRepository.GetDirectConversationAsync(
            ownerUserId,
            request.ContactUserId
        );

        if (existingConversation != null)
        {
            return new ConversationDto(
                existingConversation.Id,
                request.ContactUserId,
                existingConversation.CreatedAt,
                existingConversation.LastMessageAt
            );
        }

        var conversationId = Guid.NewGuid();

        var newConversation = new Conversation
        {
            Id = conversationId,
            IsGroup = false,
            CreatedAt = DateTime.UtcNow,

            Participants =
            [
                new ConversationParticipant { UserId = ownerUserId },
                new ConversationParticipant { UserId = request.ContactUserId }
            ],

            EncryptionKey = Convert.ToBase64String(
                E2EEncryptionService.GenerateKey()
            )
        };

        await conversationRepository.CreateAsync(newConversation);

        return new ConversationDto(
            conversationId,
            request.ContactUserId,
            newConversation.CreatedAt,
            null
        );
    }
}