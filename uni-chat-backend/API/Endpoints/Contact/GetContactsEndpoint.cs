using MediatR;
using Microsoft.AspNetCore.Mvc;
using uni_chat_backend.Features.Contacts.GetContacts;

namespace uni_chat_backend.API.Endpoints.Contact;

public static class GetContactsEndpoint
{
    public static void MapGetContactsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/contacts", async (
            [AsParameters] GetContactsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(query, cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("Contacts")
        .RequireAuthorization();
    }
}