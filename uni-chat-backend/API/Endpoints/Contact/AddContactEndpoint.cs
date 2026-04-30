using MediatR;
using Microsoft.AspNetCore.Mvc;
using uni_chat_backend.Features.Contacts.AddContact;

namespace uni_chat_backend.API.Endpoints.Contact;

public static class AddContactEndpoint
{
    public static void MapAddContactEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/contacts", async (
            [FromBody] AddContactCommand command,
            IMediator mediator) =>
        {
            var result = await mediator.Send(command);

            return Results.Ok(new
            {
                message = result
            });
        })
        .WithTags("Contacts")
        .RequireAuthorization();
    }
}