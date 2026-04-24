namespace uni_chat_backend.Domain.Entities;

public class MessageDelivery
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime DeliveredAt { get; set; }
}
