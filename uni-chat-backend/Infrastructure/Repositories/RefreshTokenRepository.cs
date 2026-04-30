using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class RefreshTokenRepository(IMongoCollections mongoCollections) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection = mongoCollections.RefreshTokens;

    public async Task CreateAsync(RefreshToken token) => await _collection.InsertOneAsync(token);

    public async Task ReplaceAsync(Guid userId, RefreshToken newToken)
    {
        await _collection.DeleteManyAsync(x => x.UserId == userId);

        await _collection.InsertOneAsync(newToken);
    }
}