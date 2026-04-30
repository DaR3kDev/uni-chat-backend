using MediatR;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;

namespace uni_chat_backend.Features.Auth.Register;

public class RegisterHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService
    ) : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly TokenService _tokenService = tokenService;

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLower();
        var username = request.Username.Trim().ToLower();

        var existingUser = await _userRepository.GetByEmailAsync(email);
       
        if (existingUser is not null)
            throw new InvalidOperationException("El email ya está en uso");

        var existingUsername = await _userRepository.GetByUsernameAsync(username);
        
        if (existingUsername is not null)
            throw new InvalidOperationException("El nombre de usuario ya está en uso");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Phone = request.Phone,
            Email = email,
            Username = username,
            IsOnline = true,
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

