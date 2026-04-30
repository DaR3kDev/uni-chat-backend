using MediatR;
using uni_chat_backend.Features.Conversations.JoinConversation;

namespace uni_chat_backend.API.Endpoints.Conversations;

public static class JoinConversationEndpoint
{
    public static void MapJoinConversationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conversations/{conversationId:guid}/join", async (
            Guid conversationId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(
                new JoinConversationCommand(conversationId),
                ct
            );

            return Results.Ok(result);
        })
        .WithTags("Conversations")
        .RequireAuthorization();
    }
}