using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using StackExchange.Redis;

namespace uni_chat_backend.Features.Auth.Logout;

public sealed class LogoutCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenRepository refreshTokenRepository,
    IConnectionMultiplexer redis
) : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IConnectionMultiplexer _redis = redis;

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

        var userId = httpContext.User?.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            var db = _redis.GetDatabase();

            await db.KeyDeleteAsync($"session:{userId}");
            await db.StringSetAsync($"user:{userId}:online", "false");
        }

        httpContext.Response.Cookies.Delete("refreshToken");

        return Unit.Value;
    }
}