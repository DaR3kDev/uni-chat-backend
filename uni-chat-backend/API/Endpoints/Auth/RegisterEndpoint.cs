using MediatR;
using Microsoft.AspNetCore.Http;
using uni_chat_backend.Features.Auth.Register;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
            RegisterCommand command,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);

            httpContext.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new
            {
                mensaje = "Usuario registrado correctamente",
                accessToken = result.AccessToken
            });
        })
        .WithTags("Auth");
    }
}