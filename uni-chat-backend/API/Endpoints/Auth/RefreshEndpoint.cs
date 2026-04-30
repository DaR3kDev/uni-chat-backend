using MediatR;
using uni_chat_backend.Features.Auth.Refresh;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class RefreshEndpoint
{
    public static void MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RefreshCommand(), cancellationToken);
            return Results.Ok(result);
        })
        .WithTags("Auth");
    }
}