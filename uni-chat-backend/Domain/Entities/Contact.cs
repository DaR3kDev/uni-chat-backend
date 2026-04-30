using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace uni_chat_backend.Domain.Entities;

public class Contact
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid OwnerUserId { get; set; } 

    [BsonRepresentation(BsonType.String)]
    public Guid ContactUserId { get; set; }

    public string? Alias { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsBlocked { get; set; } = false;
}

