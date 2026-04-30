using MediatR;
using uni_chat_backend.Features.Conversations.GetConversations;

namespace uni_chat_backend.API.Endpoints.Conversations;

public static class GetConversationsEndpoint
{
    public static void MapGetConversationsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/conversations", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetConversationsQuery(), ct);
            return Results.Ok(result);
        })
        .WithTags("Conversations")
        .RequireAuthorization();
    }
}