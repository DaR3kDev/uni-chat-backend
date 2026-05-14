namespace uni_chat_backend.Features.Auth.Register.Interfaces;

public interface IUserRegistrationValidator
{
    Task ValidateAsync(string email, string username);
}