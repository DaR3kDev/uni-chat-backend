using MediatR;
using uni_chat_backend.Features.Auth.Refresh.Services;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Refresh;

public class RefreshHandler(
    RefreshService service
) : IRequestHandler<RefreshCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        return await service.RefreshAsync(
            cancellationToken
        );
    }
}
