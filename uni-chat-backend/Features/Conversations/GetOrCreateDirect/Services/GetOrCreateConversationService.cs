using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Contracts;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect.Services;

public class GetOrCreateConversationService(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IGetOrCreateConversationCache cache,
    ILogger<GetOrCreateConversationService> logger) : IGetOrCreateConversationService
{
    public async Task<ConversationDto> ExecuteAsync(GetOrCreateConversationCommand request,
        CancellationToken cancellationToken = default)
    {
        var ownerUserId = currentUser.UserId ?? throw new UnauthorizedException("Usuario no autenticado");

        logger.LogInformation(
            "Obteniendo o creando conversación directa. OwnerUserId: {OwnerUserId}, ContactUserId: {ContactUserId}",
            ownerUserId, request.ContactUserId);

        var cachedConversation = await cache.GetAsync(ownerUserId, request.ContactUserId, cancellationToken);

        if (cachedConversation is not null)
        {
            logger.LogInformation("Conversación directa obtenida desde caché. ConversationId: {ConversationId}",
                cachedConversation.Id);

            return cachedConversation;
        }

        logger.LogInformation("Cache MISS de conversación directa. Consultando base de datos.");

        var existingConversation =
            await conversationRepository.GetDirectConversationAsync(ownerUserId, request.ContactUserId);

        if (existingConversation is not null)
        {
            logger.LogInformation("Conversación directa existente encontrada. ConversationId: {ConversationId}",
                existingConversation.Id);

            var existingDto = new ConversationDto(existingConversation.Id, request.ContactUserId,
                existingConversation.CreatedAt, existingConversation.LastMessageAt);

            await cache.SetAsync(ownerUserId, request.ContactUserId, existingDto, cancellationToken);

            return existingDto;
        }

        logger.LogInformation("No existe conversación directa. Creando nueva conversación.");

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
            EncryptionKey = Convert.ToBase64String(E2EEncryptionService.GenerateKey())
        };

        await conversationRepository.CreateAsync(newConversation);

        logger.LogInformation("Nueva conversación creada correctamente. ConversationId: {ConversationId}",
            conversationId);

        var response = new ConversationDto(conversationId, request.ContactUserId, newConversation.CreatedAt, null);

        await cache.SetAsync(ownerUserId, request.ContactUserId, response, cancellationToken);

        await cache.RemoveConversationsAsync(ownerUserId, cancellationToken);

        logger.LogInformation("Caché de conversaciones invalidada para usuario: {UserId}", ownerUserId);

        return response;
    }
}

