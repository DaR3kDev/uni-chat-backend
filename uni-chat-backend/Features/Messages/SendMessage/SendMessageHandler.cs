using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Domain.Enums;
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

        var conversation = await conversationRepository.GetByIdAsync(request.ConversationId)
            ?? throw new InvalidOperationException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == senderId && !p.IsBanned))
            throw new InvalidOperationException("No perteneces a esta conversación");

        string? encryptedContent = null;
        if (request.Type == MessageType.TEXT && !string.IsNullOrWhiteSpace(request.Content))
        {
            var key = await conversationRepository.GetEncryptionKeyAsync(request.ConversationId);
            encryptedContent = E2EEncryptionService.Encrypt(
                request.Content,
                Convert.FromBase64String(key)
            );
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = senderId,
            Content = encryptedContent, 
            FileUrl = request.FileUrl, 
            FileName = request.FileName, 
            Type = request.Type,
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
            request.Content,   
            request.FileUrl, 
            request.FileName, 
            message.Type,
            message.CreatedAt
        );
    }
}