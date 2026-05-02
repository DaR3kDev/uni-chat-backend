using MediatR;
using uni_chat_backend.Infrastructure.Repositories;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public class GetConversationsHandler(
    IUserRepository userRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetConversationsQuery, List<GetConversationsResult>>
{
    public async Task<List<GetConversationsResult>> Handle(
    GetConversationsQuery request,
    CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId
            ?? throw new UnauthorizedAccessException("No autorizado");

        var conversations = await conversationRepository
            .GetUserConversationsAsync(userId);

        var tasks = conversations.Select(async conversation =>
        {
            var participant = conversation.Participants
                .FirstOrDefault(p => p.UserId != userId);

            if (participant is null)
                return null;

            var user = await userRepository.GetByIdAsync(participant.UserId);

            if (user is null)
                return null;

            return new GetConversationsResult(
                conversation.Id,
                user.Id,
                user.Username,
                user.IsOnline,
                user.LastSeen,
                conversation.CreatedAt,
                conversation.LastMessageAt
            );
        });

        var results = await Task.WhenAll(tasks);

        return [.. results
            .Where(x => x is not null)
            .Select(x => x!)];
    }
}