using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Features.Auth.Register;
using uni_chat_backend.Features.Auth.Shared;

namespace uni_chat_backend.API.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        try
        {
            app.MapPost("/api/auth/register", async (
            RegisterCommand command,
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
                    message = "Usuario registrado correctamente",
                    accessToken = result.AccessToken
                });
            });
        }
        catch (Exception ex)
        {
            Results.Conflict(new
            {
                message = ex.Message
            });
        }
    }
}