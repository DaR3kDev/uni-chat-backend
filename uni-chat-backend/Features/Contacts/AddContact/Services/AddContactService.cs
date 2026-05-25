using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Contacts.AddContact.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.AddContact.Services;

public class AddContactService(
    IUserRepository userRepository,
    IContactRepository contactRepository,
    ICurrentUserService currentUser,
    IContactCache contactCache,
    ILogger<AddContactService> logger) : IAddContactService
{
    public async Task<string> AddAsync(AddContactCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando proceso para agregar contacto con teléfono: {Phone}", request.Phone);

        var ownerUserId = currentUser.UserId ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation("Usuario autenticado: {UserId}", ownerUserId);

        var contactUser = await userRepository.GetByPhoneAsync(request.Phone);

        if (contactUser is null)
        {
            logger.LogWarning("No se encontró usuario con teléfono: {Phone}", request.Phone);

            throw new NotFoundException("Usuario no encontrado");
        }

        logger.LogInformation("Usuario contacto encontrado: {ContactUserId}", contactUser.Id);

        if (contactUser.Id == ownerUserId)
        {
            logger.LogWarning("El usuario {UserId} intentó agregarse a sí mismo", ownerUserId);

            throw new BadRequestException("No puedes agregarte a ti mismo como contacto");
        }

        var exists = await contactRepository.ExistsAsync(ownerUserId, contactUser.Id);

        if (exists)
        {
            logger.LogWarning("El contacto ya existe entre {OwnerUserId} y {ContactUserId}", ownerUserId,
                contactUser.Id);

            throw new BadRequestException("El contacto ya existe");
        }

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            ContactUserId = contactUser.Id,
            Alias = request.Alias,
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };

        logger.LogInformation("Creando nuevo contacto con ID: {ContactId}", contact.Id);

        await contactRepository.CreateAsync(contact);

        logger.LogInformation("Contacto creado correctamente");

        await contactCache.IncrementContactsVersionAsync(ownerUserId);

        logger.LogInformation("Cache de contactos actualizada para usuario: {UserId}", ownerUserId);

        logger.LogInformation("Proceso de agregar contacto finalizado correctamente");

        return "Contacto agregado correctamente";
    }
}
