using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Login.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Auth.Login.Validators;

public class UserLoginValidator(
    IUserRepository userRepository
) : IUserLoginValidator
{
    public async Task<User> ValidateAsync(string phone)
    {
        return await userRepository.GetByPhoneAsync(phone)
               ?? throw new NotFoundException(
                   "Usuario no encontrado"
               );
    }
}
