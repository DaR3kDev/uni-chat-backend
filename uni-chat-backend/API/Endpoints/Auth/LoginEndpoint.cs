using MediatR;
using Microsoft.AspNetCore.Http;
using uni_chat_backend.Features.Auth.Login;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginCommand command,
            IMediator mediator,
            HttpContext httpContext) =>
        {
            var result = await mediator.Send(command);

            httpContext.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new
            {
                message = "Login exitoso",
                accessToken = result.AccessToken
            });
        });
    }
}