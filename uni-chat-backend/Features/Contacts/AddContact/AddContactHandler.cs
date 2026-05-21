using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.AddContact;

public class AddContactHandler(
    IUserRepository userRepository,
    IContactRepository contactRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<AddContactCommand, string>
{
    public async Task<string> Handle(
        AddContactCommand request,
        CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
                          ?? throw new UnauthorizedException("No autorizado");

        var contactUser = await userRepository.GetByPhoneAsync(request.Phone)
                          ?? throw new NotFoundException("Usuario no encontrado");

        if (contactUser.Id == ownerUserId)
            throw new BadRequestException("No puedes agregarte a ti mismo como contacto");

        var exists = await contactRepository.ExistsAsync(
            ownerUserId,
            contactUser.Id
        );

        if (exists)
            throw new BadRequestException("El contacto ya existe");

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

        var db = redis.GetDatabase();

        await db.StringIncrementAsync($"contacts:{ownerUserId}:version");

        return "Contacto agregado correctamente";
    }
}
