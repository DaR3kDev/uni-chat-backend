using MediatR;
using uni_chat_backend.Features.Messages.SendMessage;
using uni_chat_backend.Features.Messages.UploadFile;

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

        app.MapPost("/api/messages/upload", async (HttpRequest httpRequest, IMediator mediator) =>
            {
                if (!httpRequest.Form.Files.Any())
                    return Results.BadRequest("No se envió archivo");

                var file = httpRequest.Form.Files[0];
                var result = await mediator.Send(new UploadFileCommand(file));
                return Results.Ok(result);
            })
            .WithName("UploadFile")
            .WithTags("Messages")
            .RequireAuthorization();
    }
}