using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.DeleteMessage;

public class DeleteMessageHandler(
    IMessageRepository messageRepository,
    ICurrentUserService currentUser
) : IRequestHandler<DeleteMessageCommand>
{
    public async Task Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autenticado");

        var message = await messageRepository.GetByIdAsync(request.MessageId)
            ?? throw new InvalidOperationException("Mensaje no existe");

        if (message.SenderId != userId)
            throw new UnauthorizedAccessException("No puedes eliminar este mensaje");

        await messageRepository.MarkAsDeletedAsync(request.MessageId);
    }
}