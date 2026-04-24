using uni_chat_backend.API.Endpoints.Users;

namespace uni_chat_backend.API;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapCreateUser();
    }
}