using MediatR;
using uni_chat_backend.Features.Messages.SendMessage;

namespace uni_chat_backend.API.Endpoints.Messages;

public static class MessagesEndpoints
{
    public static void MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/messages/send", async (SendMessageCommand request, IMediator mediator) =>
        {
            var result = await mediator.Send(request);
            return Results.Ok(result);
        })
        .WithName("SendMessage")
        .WithTags("Messages")
        .RequireAuthorization();
    }
}   