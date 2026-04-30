using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class UserRepository(IMongoCollections mongoCollections) : IUserRepository
{
    private readonly IMongoCollection<User> _users = mongoCollections.Users;

    public Task CreateAsync(User user) =>
        _users.InsertOneAsync(user);
    public async Task<User?> GetByIdAsync(Guid id) =>
       await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task<User?> GetByPhoneAsync(string phone) =>
        await _users.Find(u => u.Phone == phone).FirstOrDefaultAsync();

    public Task<List<User>> GetByIdsAsync(List<Guid> ids)
    {
        if (ids is null || ids.Count == 0)
            return Task.FromResult(new List<User>());

        return _users.Find(u => ids.Contains(u.Id)).ToListAsync();
    }

    public Task SetOnlineAsync(Guid userId) =>
        _users.UpdateOneAsync(
            u => u.Id == userId,
            Builders<User>.Update.Set(u => u.IsOnline, true)
        );

    public Task SetOfflineAsync(Guid userId) =>
        _users.UpdateOneAsync(
            u => u.Id == userId,
            Builders<User>.Update
                .Set(u => u.IsOnline, false)
                .Set(u => u.LastSeen, DateTime.UtcNow)
        );
}