using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public class DeleteContactHandler(
    IContactRepository contactRepository,
    ICurrentUserService currentUser
) : IRequestHandler<DeleteContactCommand, string>
{
    public async Task<string> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
            ?? throw new Exception("No autorizado");

        var contact = await contactRepository.GetByIdAsync(request.ContactId) ?? throw new Exception("Contacto no encontrado");

        if (contact.OwnerUserId != ownerUserId)
            throw new Exception("No tienes permiso para eliminar este contacto");

        await contactRepository.DeleteAsync(request.ContactId);

        return "Contacto eliminado correctamente";
    }
}