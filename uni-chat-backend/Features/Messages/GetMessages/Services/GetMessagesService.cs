using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Features.Messages.GetMessages.Contracts;
using uni_chat_backend.Features.Messages.GetMessages.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.GetMessages.Services;

public class GetMessagesService(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IGetMessagesCache cache,
    ILogger<GetMessagesService> logger) : IGetMessagesService
{
    public async Task<List<GetMessagesResult>> ExecuteAsync(GetMessagesQuery request,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("No autenticado");

        logger.LogInformation("Obteniendo mensajes de conversación. UserId: {UserId}, ConversationId: {ConversationId}",
            userId, request.ConversationId);

        var cachedMessages = await cache.GetAsync(request.ConversationId, cancellationToken);

        if (cachedMessages is not null)
        {
            logger.LogInformation("Mensajes obtenidos desde caché. Count: {Count}, ConversationId: {ConversationId}",
                cachedMessages.Count, request.ConversationId);

            return cachedMessages;
        }

        logger.LogInformation("Cache MISS de mensajes. Consultando base de datos. ConversationId: {ConversationId}",
            request.ConversationId);

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId) ??
                           throw new NotFoundException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == userId))
        {
            logger.LogWarning("Usuario sin acceso a conversación. UserId: {UserId}, ConversationId: {ConversationId}",
                userId, request.ConversationId);

            throw new ForbiddenException("No perteneces a esta conversación");
        }

        var messages = await messageRepository.GetByConversationIdAsync(request.ConversationId);

        logger.LogInformation("Mensajes encontrados en base de datos. Count: {Count}, ConversationId: {ConversationId}",
            messages.Count, request.ConversationId);

        var key = await conversationRepository.GetEncryptionKeyAsync(request.ConversationId);

        var aesKey = Convert.FromBase64String(key);

        var response = messages.Select(message => new GetMessagesResult(
            message.Id,
            message.ConversationId,
            message.SenderId,

            message.Type == MessageType.TEXT &&
            !string.IsNullOrWhiteSpace(message.Content)
                ? E2EEncryptionService.Decrypt(message.Content, aesKey)
                : null,

            !string.IsNullOrWhiteSpace(message.FileUrl)
                ? E2EEncryptionService.Decrypt(message.FileUrl, aesKey)
                : null,

            !string.IsNullOrWhiteSpace(message.FileName)
                ? E2EEncryptionService.Decrypt(message.FileName, aesKey)
                : null,

            message.Type,
            message.CreatedAt
        )).ToList();

        await cache.SetAsync(request.ConversationId, response, cancellationToken);

        logger.LogInformation("Mensajes almacenados en caché. Count: {Count}, ConversationId: {ConversationId}",
            response.Count, request.ConversationId);

        return response;
    }
}

