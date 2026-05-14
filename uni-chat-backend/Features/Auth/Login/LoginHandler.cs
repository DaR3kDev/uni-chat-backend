using MediatR;
using uni_chat_backend.Features.Auth.Login.Services;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Login;

public class LoginHandler(LoginService service)
    : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        return await service.LoginAsync(request, cancellationToken);
    }
}