using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.AddContact;

public class AddContactHandler(
    IUserRepository userRepository,
    IContactRepository contactRepository,
    ICurrentUserService currentUser
) : IRequestHandler<AddContactCommand, string>
{
    public async Task<string> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autorizado");

        var contactUser = await userRepository.GetByPhoneAsync(request.Phone) ?? throw new KeyNotFoundException("Usuario no encontrado");
        
        if (contactUser.Id == ownerUserId)
            throw new InvalidOperationException("No puedes agregarte a ti mismo como contacto");

        var exists = await contactRepository.ExistsAsync(ownerUserId, contactUser.Id);

        if (exists)
            throw new InvalidOperationException("El contacto ya existe");

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            ContactUserId = contactUser.Id,
            Alias = request.Alias,
            CreatedAt = DateTime.UtcNow,
            IsBlocked = false
        };

        await contactRepository.CreateAsync(contact);

        return "Contacto agregado correctamente";
    }
}