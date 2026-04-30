using MediatR;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public record GetConversationsQuery() : IRequest<List<GetConversationsResult>>;   
    

