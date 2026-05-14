using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class RefreshTokenRepository(IMongoCollections mongoCollections) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection = mongoCollections.RefreshTokens;

    public async Task CreateAsync(RefreshToken token)
    {
        await _collection.InsertOneAsync(token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _collection
            .Find(x => x.Token == token)
            .FirstOrDefaultAsync();
    }

    public async Task RevokeAsync(Guid id, string? replacedByToken = null)
    {
        var update = Builders<RefreshToken>.Update
            .Set(x => x.IsRevoked, true)
            .Set(x => x.RevokedAt, DateTime.UtcNow)
            .Set(x => x.ReplacedByToken, replacedByToken);

        await _collection.UpdateOneAsync(
            x => x.Id == id,
            update
        );
    }

    public async Task RevokeAllByUserIdAsync(Guid userId)
    {
        var update = Builders<RefreshToken>.Update
            .Set(x => x.IsRevoked, true)
            .Set(x => x.RevokedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            x => x.UserId == userId && !x.IsRevoked,
            update
        );
    }
}