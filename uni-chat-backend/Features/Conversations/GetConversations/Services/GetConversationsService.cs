using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Conversations.GetConversations.Contracts;
using uni_chat_backend.Features.Conversations.GetConversations.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations.Services;

public class GetConversationsService(
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IGetConversationsCache cache,
    ILogger<GetConversationsService> logger) : IGetConversationsService
{
    public async Task<List<GetConversationsResult>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation("Obteniendo conversaciones del usuario: {UserId}", userId);

        var cached = await cache.GetAsync(userId, cancellationToken);

        if (cached is not null)
        {
            logger.LogInformation("Conversaciones obtenidas desde caché. Count: {Count}, UserId: {UserId}",
                cached.Count, userId);

            return cached;
        }

        logger.LogInformation("Cache MISS de conversaciones. Consultando base de datos para usuario: {UserId}", userId);

        var conversations = await conversationRepository.GetUserConversationsAsync(userId);

        logger.LogInformation("Conversaciones encontradas en base de datos: {Count}, UserId: {UserId}",
            conversations.Count, userId);

        var participantIds = conversations.SelectMany(x => x.Participants)
            .Where(x => x.UserId != userId)
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        logger.LogInformation("Obteniendo participantes. Count: {Count}", participantIds.Count);

        var users = await userRepository.GetByIdsAsync(participantIds);

        var usersMap = users.ToDictionary(x => x.Id);

        var onlineUsers = await cache.GetOnlineStatusesAsync(participantIds, cancellationToken);

        var response = conversations.Select(conversation =>
            {
                var participant = conversation.Participants.FirstOrDefault(x => x.UserId != userId);

                if (participant is null) return null;

                if (!usersMap.TryGetValue(participant.UserId, out var user)) return null;

                onlineUsers.TryGetValue(user.Id, out var isOnline);

                return new GetConversationsResult(conversation.Id, user.Id, user.Username, isOnline, user.LastSeen,
                    conversation.CreatedAt, conversation.LastMessageAt);
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        await cache.SetAsync(userId, response, cancellationToken);

        logger.LogInformation("Conversaciones almacenadas en caché. Count: {Count}, UserId: {UserId}", response.Count,
            userId);

        logger.LogInformation("Obtención de conversaciones finalizada correctamente. Count: {Count}, UserId: {UserId}",
            response.Count, userId);

        return response;
    }
}
