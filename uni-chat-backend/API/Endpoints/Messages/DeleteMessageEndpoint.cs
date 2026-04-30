using MediatR;
using uni_chat_backend.Features.Messages.DeleteMessage;

namespace uni_chat_backend.API.Endpoints.Messages;

public static class DeleteMessageEndpoint
{
    public static void MapDeleteMessageEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/messages/{messageId:guid}",
            async (
                Guid messageId,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new DeleteMessageCommand(messageId);

                await mediator.Send(command, cancellationToken);

                return Results.NoContent();
            })
            .WithName("DeleteMessage")
            .WithTags("Messages")
            .RequireAuthorization();
    }
}