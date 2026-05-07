using MediatR;
using StackExchange.Redis;
using System.Text.Json;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public class GetConversationsHandler(
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<GetConversationsQuery, List<GetConversationsResult>>
{
    public async Task<List<GetConversationsResult>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedException("No autorizado");

        var db = redis.GetDatabase();
        var cacheKey = $"conversations:{userId}";
        var cached = await db.StringGetAsync(cacheKey);


        if (cached.HasValue)
        {
            var json = cached.ToString();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var cachedResponse =
                    JsonSerializer.Deserialize<List<GetConversationsResult>>(json);

                if (cachedResponse is not null)
                    return cachedResponse;
            }
        }

        var conversations =
            await conversationRepository.GetUserConversationsAsync(userId);

        var participantIds = conversations
            .SelectMany(c => c.Participants)
            .Where(p => p.UserId != userId)
            .Select(p => p.UserId)
            .Distinct()
            .ToList();

        var users = await userRepository.GetByIdsAsync(participantIds);

        var usersMap = users.ToDictionary(x => x.Id);

        var tasks = conversations.Select(async conversation =>
        {
            var participant = conversation.Participants
                .FirstOrDefault(p => p.UserId != userId);

            if (participant is null)
                return null;

            if (!usersMap.TryGetValue(participant.UserId, out var user))
                return null;

            var isOnline =
                await db.StringGetAsync($"user:{user.Id}:online");

            return new GetConversationsResult(
                conversation.Id,
                user.Id,
                user.Username,
                isOnline == "true",
                user.LastSeen,
                conversation.CreatedAt,
                conversation.LastMessageAt
            );
        });

        var response = (await Task.WhenAll(tasks))
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        await db.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromMinutes(5)
        );

        return response;
    }
}