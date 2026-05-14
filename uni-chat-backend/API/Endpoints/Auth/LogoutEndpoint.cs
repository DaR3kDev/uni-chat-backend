using MediatR;
using uni_chat_backend.Features.Auth.Logout;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class LogoutEndpoint
{
    public static void MapLogoutEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/logout", async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                await mediator.Send(new LogoutCommand(), cancellationToken);

                return Results.Ok(new { message = "Cierre de sesión realizado correctamente" });
            })
            .RequireAuthorization();
    }
}