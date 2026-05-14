using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Auth.Register.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Auth.Register.Validators;

public class UserRegistrationValidator(IUserRepository userRepository)
    : IUserRegistrationValidator
{
    public async Task ValidateAsync(string email, string username)
    {
        var existingUser = await userRepository.GetByEmailAsync(email);

        if (existingUser is not null)
            throw new ConflictException("El correo electrónico ya está en uso");

        var existingUsername = await userRepository.GetByUsernameAsync(username);

        if (existingUsername is not null)
            throw new ConflictException("El nombre de usuario ya está en uso");
    }
}