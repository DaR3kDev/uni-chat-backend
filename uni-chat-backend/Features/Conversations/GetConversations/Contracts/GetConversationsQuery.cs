using MediatR;

namespace uni_chat_backend.Features.Conversations.GetConversations.Contracts;

public record GetConversationsQuery : IRequest<List<GetConversationsResult>>;
