using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;
using uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;
using Wolverine;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Services;

public class JoinConversationService(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IJoinConversationCache cache,
    IMessageBus bus,
    ILogger<JoinConversationService> logger) : IJoinConversationService
{
    public async Task<JoinConversationResult> ExecuteAsync(JoinConversationCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation(
            "Usuario intentando unirse a conversación. UserId: {UserId}, ConversationId: {ConversationId}", userId,
            request.ConversationId);

        var isMember = await conversationRepository.IsUserInConversationAsync(request.ConversationId, userId);

        if (!isMember)
        {
            logger.LogWarning("Acceso denegado a conversación. UserId: {UserId}, ConversationId: {ConversationId}",
                userId, request.ConversationId);

            throw new ForbiddenException("No tienes acceso a esta conversación");
        }

        logger.LogInformation(
            "Usuario autorizado para conversación. UserId: {UserId}, ConversationId: {ConversationId}", userId,
            request.ConversationId);

        await cache.SetUserOnlineAsync(userId, cancellationToken);

        logger.LogInformation("Usuario marcado como online. UserId: {UserId}", userId);

        await cache.SetActiveConversationAsync(userId, request.ConversationId, cancellationToken);

        await bus.PublishAsync(new UserJoinedConversation(request.ConversationId, userId, DateTime.UtcNow));

        logger.LogInformation("Conversación activa actualizada. UserId: {UserId}, ConversationId: {ConversationId}",
            userId, request.ConversationId);

        return new JoinConversationResult(request.ConversationId, true);
    }
}
