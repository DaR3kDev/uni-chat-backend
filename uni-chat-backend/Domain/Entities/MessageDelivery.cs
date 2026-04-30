using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace uni_chat_backend.Domain.Entities;

public class MessageDelivery
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime DeliveredAt { get; set; }
}
