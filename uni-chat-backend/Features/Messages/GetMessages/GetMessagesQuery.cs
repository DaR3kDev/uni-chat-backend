using MediatR;

namespace uni_chat_backend.Features.Messages.GetMessages;

public record GetMessagesQuery
(
    Guid ConversationId
) : IRequest<List<GetMessagesResult>>;