namespace uni_chat_backend.Domain.Entities;

public class MessageRead
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }
}