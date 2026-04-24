using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace uni_chat_backend.Domain.Entities;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public bool IsGroup { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastMessageAt { get; set; }

    //Relationships
    public List<ConversationParticipant> Participants { get; set; } = [];
}

