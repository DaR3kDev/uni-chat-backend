using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByPhoneAsync(string phone);
    Task CreateAsync(User user);
}

