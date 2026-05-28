using MediatR;
using Microsoft.Extensions.Logging;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Conversations.GetConversations.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public class GetConversationsHandler(
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IGetConversationsCache cache,
    ILogger<GetConversationsHandler> logger
) : IRequestHandler<GetConversationsQuery, List<GetConversationsResult>>
{
    public async Task<List<GetConversationsResult>> Handle(
        GetConversationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation(
            "Iniciando obtención de conversaciones para usuario: {UserId}",
            userId
        );

        var cached = await cache.GetAsync(userId);

        if (cached is not null)
        {
            logger.LogInformation(
                "Conversaciones obtenidas desde caché. Count: {Count}, UserId: {UserId}",
                cached.Count,
                userId
            );

            return cached;
        }

        logger.LogInformation(
            "Cache MISS de conversaciones. Consultando base de datos para usuario: {UserId}",
            userId
        );

        var conversations =
            await conversationRepository.GetUserConversationsAsync(userId);

        logger.LogInformation(
            "Conversaciones encontradas en base de datos: {Count}, UserId: {UserId}",
            conversations.Count,
            userId
        );

        var participantIds = conversations
            .SelectMany(c => c.Participants)
            .Where(p => p.UserId != userId)
            .Select(p => p.UserId)
            .Distinct()
            .ToList();

        logger.LogInformation(
            "Obteniendo información de participantes. Participants: {Count}",
            participantIds.Count
        );

        var users = await userRepository.GetByIdsAsync(participantIds);

        var usersMap = users.ToDictionary(x => x.Id);

        var onlineUsers = await cache.GetOnlineStatusesAsync(participantIds);

        var response = conversations
            .Select(conversation =>
            {
                var participant = conversation.Participants
                    .FirstOrDefault(p => p.UserId != userId);

                if (participant is null)
                    return null;

                if (!usersMap.TryGetValue(participant.UserId, out var user))
                    return null;

                onlineUsers.TryGetValue(user.Id, out var isOnline);

                return new GetConversationsResult(
                    conversation.Id,
                    user.Id,
                    user.Username,
                    isOnline,
                    user.LastSeen,
                    conversation.CreatedAt,
                    conversation.LastMessageAt
                );
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .ToList();

        await cache.SetAsync(userId, response);

        logger.LogInformation(
            "Conversaciones almacenadas en caché. Count: {Count}, UserId: {UserId}",
            response.Count,
            userId
        );

        logger.LogInformation(
            "Proceso de obtención de conversaciones finalizado correctamente. Count: {Count}, UserId: {UserId}",
            response.Count,
            userId
        );

        return response;
    }
}
