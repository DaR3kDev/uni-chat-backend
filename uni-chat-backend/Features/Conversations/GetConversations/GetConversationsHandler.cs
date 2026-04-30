using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public class GetConversationsHandler(
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

        var conversations = await conversationRepository.GetUserConversationsAsync(userId);

        return [.. conversations.Select(c =>
        {
            var contact = c.Participants
                .FirstOrDefault(p => p.UserId != userId);

            return new GetConversationsResult(
                c.Id,
                contact?.UserId ?? Guid.Empty,
                c.CreatedAt,
                c.LastMessageAt
            );
        })];
    }
}