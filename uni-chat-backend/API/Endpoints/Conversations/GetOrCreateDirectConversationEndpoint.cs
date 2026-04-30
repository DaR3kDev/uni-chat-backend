using MediatR;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect;

namespace uni_chat_backend.API.Endpoints.Conversations;

public static class GetOrCreateDirectConversationEndpoint
{
    public static void MapGetOrCreateDirectConversationEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/conversations/direct", async (
            GetOrCreateConversationCommand command,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var conversationId = await mediator.Send(command, cancellationToken);

            return Results.Ok(new
            {
                conversationId
            });
        })
        .WithTags("Conversations")
        .RequireAuthorization();
    }
}