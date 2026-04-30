using MediatR;
using uni_chat_backend.Features.Auth.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.API.Exceptions;

namespace uni_chat_backend.Features.Auth.Login;

public class LoginHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    TokenService tokenService
) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByPhoneAsync(request.Phone) ?? throw new NotFoundException("User not found");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user.Id);

        await refreshTokenRepository.RevokeAsync(user.Id, refreshToken.Token);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }
}