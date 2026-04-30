using MediatR;
using uni_chat_backend.Features.Messages.GetMessages;

namespace uni_chat_backend.API.Endpoints.Messages;

public static class GetMessagesEndpoint
{
    public static void MapGetMessagesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/messages/conversation/{conversationId:guid}",
            async (
                Guid conversationId,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetMessagesQuery(conversationId);

                var result = await mediator.Send(query, cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetMessages")
            .WithTags("Messages")
            .RequireAuthorization();
    }
}