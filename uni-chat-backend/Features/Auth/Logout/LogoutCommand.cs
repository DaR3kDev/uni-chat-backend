using MediatR;

namespace uni_chat_backend.Features.Auth.Logout;

public sealed record LogoutCommand : IRequest<Unit>;