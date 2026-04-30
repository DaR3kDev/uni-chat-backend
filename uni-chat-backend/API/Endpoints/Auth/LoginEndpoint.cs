using MediatR;
using Microsoft.AspNetCore.Http;
using uni_chat_backend.Features.Auth.Login;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginCommand command,
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
                message = "Inicio de sesión exitoso",
                accessToken = result.AccessToken
            });
        })
        .WithTags("Auth");
    }
}