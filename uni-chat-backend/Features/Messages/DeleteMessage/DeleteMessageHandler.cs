using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.DeleteMessage;

public class DeleteMessageHandler(
    IMessageRepository messageRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<DeleteMessageCommand>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
                     ?? throw new UnauthorizedException("No autenticado");

        var message = await _messageRepository.GetByIdAsync(request.MessageId)
                      ?? throw new NotFoundException("Mensaje no existe");

        if (message.SenderId != userId)
            throw new ForbiddenException("No puedes eliminar este mensaje");

        await _messageRepository.MarkAsDeletedAsync(request.MessageId);

        var db = _redis.GetDatabase();

        await db.KeyDeleteAsync($"messages:{message.ConversationId}");

        await db.KeyDeleteAsync($"conversations:{userId}");
    }
}
