using MediatR;
using uni_chat_backend.Features.Auth.Register.Services;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.Features.Auth.Register;

public class RegisterHandler(RegisterService service) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return service.RegisterAsync(request, cancellationToken);
    }
}