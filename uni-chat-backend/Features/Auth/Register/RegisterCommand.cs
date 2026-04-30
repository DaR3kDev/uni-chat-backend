using MediatR;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Register;

public record RegisterCommand(
    string Username,
    string? Phone,
    string Email
) : IRequest<AuthResponse>;
