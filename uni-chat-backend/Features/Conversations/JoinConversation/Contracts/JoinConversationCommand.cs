using MediatR;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Contracts;

public record JoinConversationCommand(
    Guid ConversationId
) : IRequest<JoinConversationResult>;
