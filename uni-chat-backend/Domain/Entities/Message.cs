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
    public MessageType Type { get; set; } = MessageType.TEXT;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    public MessageStatus Status { get; set; } = MessageStatus.SENT;

    // Relationships
    public EncryptionData? Encryption { get; set; }
}
