using MediatR;
using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Features.Auth.Me;

public record MeCommand : IRequest<User>;
