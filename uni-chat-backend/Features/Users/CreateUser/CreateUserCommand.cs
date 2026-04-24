using MediatR;

namespace uni_chat_backend.Features.Users.CreateUser;

public record CreateUserCommand(
    string Email,
    string Username,
    string Password
) : IRequest<Guid>;