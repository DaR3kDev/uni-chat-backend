using MediatR;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect;

public record GetOrCreateConversationCommand
(
 Guid ContactUserId
) : IRequest<ConversationDto>;

