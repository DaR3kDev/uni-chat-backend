using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace uni_chat_backend.Domain.Entities;

public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string Token { get; set; } = default!;

    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByToken { get; set; }
}

