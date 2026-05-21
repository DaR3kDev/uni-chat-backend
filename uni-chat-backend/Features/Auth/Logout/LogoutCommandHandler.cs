using MediatR;
using uni_chat_backend.Features.Auth.Logout.Services;

namespace uni_chat_backend.Features.Auth.Logout;

public sealed class LogoutCommandHandler(LogoutService service) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await service.LogoutAsync(cancellationToken);

        return Unit.Value;
    }
}
