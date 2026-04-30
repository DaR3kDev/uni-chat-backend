using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace uni_chat_backend.Domain.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsOnline { get; set; } = false;
    public DateTime? LastSeen { get; set; }
}