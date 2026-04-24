using MediatR;
using uni_chat_backend.Features.Users.CreateUser;

namespace uni_chat_backend.API.Endpoints.Users;

public static class CreateUserEndpoint
{
    public static void MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", async (
            CreateUserDto dto,
            IMediator mediator
        ) =>
        {
            var command = new CreateUserCommand(
                dto.Email!,
                dto.UserName!,
                dto.Password!
            );

            var userId = await mediator.Send(command);

            return Results.Created($"/api/users/{userId}", new { userId });
        });
    }
}

