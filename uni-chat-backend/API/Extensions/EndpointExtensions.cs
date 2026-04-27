using uni_chat_backend.API.Endpoints.Auth;

namespace uni_chat_backend.API.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapRegisterEndpoint();
    }
}