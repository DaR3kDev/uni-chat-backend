using MediatR;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Login;

public sealed record LoginCommand(string Phone)
    : IRequest<AuthResponse>;