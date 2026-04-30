using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task CreateAsync(User user);

    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByPhoneAsync(string phone);

    Task<List<User>> GetByIdsAsync(List<Guid> ids);

    Task SetOnlineAsync(Guid userId);

    Task SetOfflineAsync(Guid userId);
}