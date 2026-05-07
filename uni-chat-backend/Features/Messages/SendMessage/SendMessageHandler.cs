using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;
using Wolverine;

namespace uni_chat_backend.Features.Messages.SendMessage;

public sealed class SendMessageHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis,
    IMessageBus bus
) : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IConnectionMultiplexer _redis = redis;
    private readonly IMessageBus _bus = bus;

    public async Task<SendMessageResult> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var senderId = _currentUser.UserId ??
            throw new UnauthorizedAccessException("Usuario no autenticado");

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId) ??
            throw new InvalidOperationException("Conversación no existe");

        var isParticipant = conversation.Participants.Any(p =>
            p.UserId == senderId &&
            !p.IsBanned);

        if (!isParticipant)
            throw new InvalidOperationException("No perteneces a esta conversación");

        string? encryptedContent = null;

        if (request.Type == MessageType.TEXT &&
            !string.IsNullOrWhiteSpace(request.Content))
        {
            var key = await _conversationRepository
                .GetEncryptionKeyAsync(request.ConversationId);

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

        await _messageRepository.CreateAsync(message);

        await _conversationRepository.UpdateLastMessageAsync(
            request.ConversationId,
            message.CreatedAt
        );

        var db = _redis.GetDatabase();

        await db.KeyDeleteAsync($"messages:{request.ConversationId}");

        await Task.WhenAll(
            conversation.Participants.Select(p =>
                db.KeyDeleteAsync($"conversations:{p.UserId}"))
        );

        await Task.WhenAll(
            conversation.Participants
                .Where(p => p.UserId != senderId)
                .Select(p =>
                    db.StringIncrementAsync(
                        $"conversation:{request.ConversationId}:unread:{p.UserId}"
                    ))
        );

        await _bus.PublishAsync(new SendMessageEvent(
            message.Id,
            message.ConversationId,
            message.SenderId,
            request.Content,
            message.Type,
            message.CreatedAt
        ));

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