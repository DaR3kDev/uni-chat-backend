using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public class DeleteContactHandler(
    IContactRepository contactRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<DeleteContactCommand, string>
{
    public async Task<string> Handle(
        DeleteContactCommand request,
        CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
                          ?? throw new UnauthorizedException("No autorizado");

        var contact = await contactRepository.GetByIdAsync(request.ContactId)
                      ?? throw new NotFoundException("Contacto no encontrado");

        if (contact.OwnerUserId != ownerUserId)
            throw new ForbiddenException("No tienes permiso para eliminar este contacto");

        await contactRepository.DeleteAsync(request.ContactId);

        var db = redis.GetDatabase();

        // invalidación limpia
        await db.StringIncrementAsync(
            $"contacts:{ownerUserId}:version"
        );

        return "Contacto eliminado correctamente";
    }
}