using MediatR;

namespace uni_chat_backend.Features.Conversations.JoinConversation;

public record JoinConversationCommand
(
    Guid ConversationId
) : IRequest<JoinConversationResult>;

