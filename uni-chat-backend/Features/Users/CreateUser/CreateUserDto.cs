namespace uni_chat_backend.Features.Users.CreateUser;

public record CreateUserDto
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
}

