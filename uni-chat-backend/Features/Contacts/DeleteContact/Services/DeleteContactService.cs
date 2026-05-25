using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.DeleteContact.Services;

public class DeleteContactService(
    IContactRepository contactRepository,
    ICurrentUserService currentUser,
    IDeleteContactCache deleteContactCache,
    ILogger<DeleteContactService> logger) : IDeleteContactService
{
    public async Task<string> DeleteAsync(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando eliminación del contacto: {ContactId}", request.ContactId);

        var ownerUserId = currentUser.UserId ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation("Usuario autenticado: {UserId}", ownerUserId);

        var contact = await contactRepository.GetByIdAsync(request.ContactId);

        if (contact is null)
        {
            logger.LogWarning("Contacto no encontrado: {ContactId}", request.ContactId);

            throw new NotFoundException("Contacto no encontrado");
        }

        logger.LogInformation("Contacto encontrado: {ContactId}", contact.Id);

        if (contact.OwnerUserId != ownerUserId)
        {
            logger.LogWarning("El usuario {UserId} intentó eliminar un contacto sin permisos", ownerUserId);

            throw new ForbiddenException("No tienes permiso para eliminar este contacto");
        }

        await contactRepository.DeleteAsync(request.ContactId);

        logger.LogInformation("Contacto eliminado correctamente: {ContactId}", request.ContactId);

        await deleteContactCache.IncrementContactsVersionAsync(ownerUserId);

        logger.LogInformation("Cache de contactos actualizada para usuario: {UserId}", ownerUserId);

        logger.LogInformation("Proceso de eliminación finalizado correctamente");

        return "Contacto eliminado correctamente";
    }
}
