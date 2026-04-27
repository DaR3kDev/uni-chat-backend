
using MediatR;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Register;

public record RegisterCommand(
    string? FirstName,
    string Username,
    string? LastName,
    string? Phone,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<AuthResponse>;
