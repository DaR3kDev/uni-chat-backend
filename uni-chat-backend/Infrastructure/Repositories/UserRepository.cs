using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class UserRepository(MongoCollections mongoCollections) : IUserRepository
{
    private readonly IMongoCollection<User> _users = mongoCollections.Users;

    public async Task CreateAsync(User user) =>
        await _users.InsertOneAsync(user);
        
    public async Task<User?> GetByEmailAsync(string email) =>
        await _users
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _users
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync();
}

