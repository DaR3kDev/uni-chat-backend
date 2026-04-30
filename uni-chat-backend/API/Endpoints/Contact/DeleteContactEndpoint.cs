using MediatR;
using uni_chat_backend.Features.Contacts.DeleteContact;

namespace uni_chat_backend.API.Endpoints.Contact;

public static class DeleteContactEndpoint
{
    public static void MapDeleteContactEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/contacts/{contactId:guid}", async (
            Guid contactId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new DeleteContactCommand(contactId),
                cancellationToken
            );

            return Results.Ok(new
            {
                message = result
            });
        })
        .WithTags("Contacts")
        .RequireAuthorization();
    }
}