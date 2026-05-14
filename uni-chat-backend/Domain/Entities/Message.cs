using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using uni_chat_backend.Domain.Enums;

namespace uni_chat_backend.Domain.Entities;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string? Content { get; set; }

    [BsonRepresentation(BsonType.String)] public MessageType Type { get; set; } = MessageType.TEXT;

    [BsonRepresentation(BsonType.String)] public MessageStatus Status { get; set; } = MessageStatus.SENT;

    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public EncryptionData? Encryption { get; set; }
}