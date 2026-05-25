using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Me.Services;

namespace uni_chat_backend.Features.Auth.Me;

public class MeHandler(MeService service)
    : IRequestHandler<MeCommand, User>
{
    public async Task<User> Handle(
        MeCommand request,
        CancellationToken cancellationToken
    )
    {
        return await service.GetCurrentUserAsync();
    }
}
