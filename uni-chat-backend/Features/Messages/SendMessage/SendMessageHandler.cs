using MediatR;
using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.SendMessage;

public class SendMessageHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser
) : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var senderId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("Usuario no autenticado");

        var conversation = await conversationRepository
            .GetByIdAsync(request.ConversationId) ?? throw new InvalidOperationException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == senderId && !p.IsBanned))
            throw new InvalidOperationException("No perteneces a esta conversación");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new HubException("El mensaje no puede estar vacío");

        var key = await conversationRepository.GetEncryptionKeyAsync(request.ConversationId);

        var encryptedContent = E2EEncryptionService.Encrypt(
            request.Content,
            Convert.FromBase64String(key)
        );

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = senderId,
            Content = encryptedContent,
            CreatedAt = DateTime.UtcNow
        };

        await messageRepository.CreateAsync(message);

        await conversationRepository.UpdateLastMessageAsync(
            request.ConversationId,
            message.CreatedAt
        );

        return new SendMessageResult(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Content,
            message.CreatedAt,
            Delivered: false
        );
    }
}