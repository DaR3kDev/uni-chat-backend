using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Login.Interfaces;

public interface IUserLoginValidator
{
    Task<User> ValidateAsync(string phone);
}
