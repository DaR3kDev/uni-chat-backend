using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Features.Auth.Logout;

public sealed class LogoutCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<Unit> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
            return Unit.Value;

        var refreshToken = httpContext.Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var tokenEntity =
                await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (tokenEntity is not null && !tokenEntity.IsRevoked)
                await _refreshTokenRepository.RevokeAsync(tokenEntity.Id);
            
        }

        httpContext.Response.Cookies.Delete("refreshToken");

        return Unit.Value;
    }
}