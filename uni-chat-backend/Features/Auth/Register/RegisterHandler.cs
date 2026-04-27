using MediatR;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Register;

public class RegisterHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService,
    Argon2PasswordHasher hasher) : IRequestHandler<RegisterCommand, AuthResponse>

{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly TokenService _tokenService = tokenService;
    private readonly Argon2PasswordHasher _hasher = hasher;

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
            throw new Exception("Las contraseñas no coinciden");

        var email = request.Email.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
            throw new Exception("El email ya está en uso");

        var existingUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUsername is not null)
            throw new Exception("El nombre de usuario ya está en uso");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Email = email,
            Username = request.Username,
            Password = _hasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.CreateAsync(refreshToken);
        
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}

