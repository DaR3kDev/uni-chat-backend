using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Features.Messages.SendMessage.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;
using Wolverine;

namespace uni_chat_backend.Features.Messages.SendMessage.Services;

public class SendMessageService(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    ISendMessageCache cache,
    IMessageBus bus,
    ILogger<SendMessageService> logger) : ISendMessageService
{
    public async Task<SendMessageResult> ExecuteAsync(SendMessageCommand request,
        CancellationToken cancellationToken = default)
    {
        var senderId = currentUser.UserId ?? throw new UnauthorizedAccessException("Usuario no autenticado");

        logger.LogInformation("Enviando mensaje. SenderId: {SenderId}, ConversationId: {ConversationId}", senderId,
            request.ConversationId);

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId) ??
                           throw new InvalidOperationException("Conversación no existe");

        logger.LogInformation("Conversación encontrada. ConversationId: {ConversationId}", conversation.Id);

        var isParticipant = conversation.Participants.Any(p => p.UserId == senderId && !p.IsBanned);

        if (!isParticipant)
        {
            logger.LogWarning(
                "Usuario sin acceso para enviar mensaje. SenderId: {SenderId}, ConversationId: {ConversationId}",
                senderId, request.ConversationId);

            throw new InvalidOperationException("No perteneces a esta conversación");
        }

        logger.LogInformation("Usuario validado correctamente en la conversación. SenderId: {SenderId}", senderId);

        string? encryptedContent = null;
        string? encryptedFileUrl = null;
        string? encryptedFileName = null;

        logger.LogInformation("Obteniendo clave de encriptación. ConversationId: {ConversationId}",
            request.ConversationId);

        var key = await conversationRepository.GetEncryptionKeyAsync(request.ConversationId);

        var encryptionKey = Convert.FromBase64String(key);

        if (request.Type == MessageType.TEXT && !string.IsNullOrWhiteSpace(request.Content))
        {
            logger.LogInformation("Encriptando contenido del mensaje. ConversationId: {ConversationId}",
                request.ConversationId);

            encryptedContent = E2EEncryptionService.Encrypt(request.Content, encryptionKey);

            logger.LogInformation("Contenido del mensaje encriptado correctamente. ConversationId: {ConversationId}",
                request.ConversationId);
        }

        if (!string.IsNullOrWhiteSpace(request.FileUrl))
        {
            logger.LogInformation("Encriptando FileUrl. ConversationId: {ConversationId}", request.ConversationId);

            encryptedFileUrl = E2EEncryptionService.Encrypt(request.FileUrl, encryptionKey);

            logger.LogInformation("FileUrl encriptado correctamente. ConversationId: {ConversationId}",
                request.ConversationId);
        }

        if (!string.IsNullOrWhiteSpace(request.FileName))
        {
            logger.LogInformation("Encriptando FileName. ConversationId: {ConversationId}", request.ConversationId);

            encryptedFileName = E2EEncryptionService.Encrypt(request.FileName, encryptionKey);

            logger.LogInformation("FileName encriptado correctamente. ConversationId: {ConversationId}",
                request.ConversationId);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = senderId,
            Content = encryptedContent,
            FileUrl = encryptedFileUrl,
            FileName = encryptedFileName,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow
        };

        logger.LogInformation("Persistiendo mensaje en base de datos. MessageId: {MessageId}", message.Id);

        await messageRepository.CreateAsync(message);

        await conversationRepository.UpdateLastMessageAsync(request.ConversationId, message.CreatedAt);

        logger.LogInformation("Mensaje almacenado correctamente. MessageId: {MessageId}", message.Id);

        logger.LogInformation("Invalidando cache de mensajes. ConversationId: {ConversationId}", conversation.Id);

        await cache.RemoveMessagesAsync(conversation.Id, cancellationToken);

        logger.LogInformation(
            "Actualizando cache de conversaciones y unread counters. ConversationId: {ConversationId}",
            conversation.Id);

        await Task.WhenAll(conversation.Participants.Select(async participant =>
        {
            logger.LogInformation("Invalidando cache de conversaciones. UserId: {UserId}", participant.UserId);

            await cache.RemoveConversationsAsync(participant.UserId, cancellationToken);

            if (participant.UserId != senderId)
            {
                logger.LogInformation(
                    "Incrementando unread messages. UserId: {UserId}, ConversationId: {ConversationId}",
                    participant.UserId, conversation.Id);

                await cache.IncrementUnreadAsync(conversation.Id, participant.UserId, cancellationToken);
            }
        }));

        logger.LogInformation("Cache invalidada correctamente. ConversationId: {ConversationId}", conversation.Id);

        logger.LogInformation("Publicando evento MessageSent. MessageId: {MessageId}", message.Id);

        await bus.PublishAsync(new MessageSent(message.Id, message.ConversationId, message.SenderId, request.Content,
            request.FileUrl, request.FileName, message.Type, message.CreatedAt));

        logger.LogInformation("Evento MessageSent publicado correctamente. MessageId: {MessageId}", message.Id);

        return new SendMessageResult(message.Id, message.ConversationId, message.SenderId, request.Content,
            request.FileUrl, request.FileName, message.Type, message.CreatedAt);
    }
}
