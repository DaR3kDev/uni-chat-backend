namespace uni_chat_backend.Infrastructure.Security.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}

