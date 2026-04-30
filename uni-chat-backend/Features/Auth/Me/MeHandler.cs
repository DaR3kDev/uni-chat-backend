using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Auth.Me;

public class MeHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService
) : IRequestHandler<MeCommand, User>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<User> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId is null)
            throw new UnauthorizedAccessException("Usuario no autenticado");

        var user = await _userRepository.GetByIdAsync(userId.Value);

        if (user is null)
            throw new Exception("Usuario no encontrado");

        return user;
    }
}

