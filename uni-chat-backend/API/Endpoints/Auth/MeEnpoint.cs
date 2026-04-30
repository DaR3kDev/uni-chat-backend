using MediatR;
using Microsoft.AspNetCore.Authorization;
using uni_chat_backend.Features.Auth.Me;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class MeEndpoint
{
    public static void MapMeEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new MeCommand(), cancellationToken);

            return Results.Ok(result);
        })
        .WithTags("Auth")
        .RequireAuthorization();
    }
}