using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.JoinConversation;

public class JoinConversationHandler(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<JoinConversationCommand, JoinConversationResult>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<JoinConversationResult> Handle(
        JoinConversationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("No autorizado");

        var isMember =
            await _conversationRepository.IsUserInConversationAsync(
                request.ConversationId,
                userId
            );

        if (!isMember)
            throw new ForbiddenException("No tienes acceso a esta conversación");

        var db = _redis.GetDatabase();

        await db.StringSetAsync(
            $"user:{userId}:online",
            "true",
            TimeSpan.FromMinutes(30)
        );

        await db.StringSetAsync(
            $"user:{userId}:active-conversation",
            request.ConversationId.ToString(),
            TimeSpan.FromMinutes(30)
        );

        return new JoinConversationResult(
            request.ConversationId,
            true
        );
    }
}