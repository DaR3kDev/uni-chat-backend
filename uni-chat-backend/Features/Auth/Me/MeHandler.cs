using MediatR;
using System.Text.Json;
using StackExchange.Redis;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Features.Auth.Me;
public class MeHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IConnectionMultiplexer redis
) : IRequestHandler<MeCommand, User>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<User> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedException("Usuario no autenticado");

        var db = _redis.GetDatabase();
        var cacheKey = $"user:{userId}";
        var cachedUser = await db.StringGetAsync(cacheKey);

        if (cachedUser.HasValue)
        {
            var cachedJson = cachedUser.ToString();

            if (!string.IsNullOrWhiteSpace(cachedJson))
            {
                var userFromCache = JsonSerializer.Deserialize<User>(cachedJson);
                if (userFromCache is not null)
                    return userFromCache;
            }
        }

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("Usuario no encontrado");

        var serializedUser = JsonSerializer.Serialize(user);

        await db.StringSetAsync(
            cacheKey,
            serializedUser,
            TimeSpan.FromMinutes(10)
        );

        return user;
    }
}