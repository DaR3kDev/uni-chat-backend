using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Users.CreateUser;

public class CreateUserHandler(IUserRepository userRepository) : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
            throw new Exception("El email ya está en uso");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            //Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return user.Id;
    }
}

