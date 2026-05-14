using uni_chat_backend.API.Extensions;
using uni_chat_backend.Infrastructure.SignalR;

namespace uni_chat_backend.API.Configuration.Middleware;

public static class EndpointConfiguration
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapHub<ChatHub>("/messages/chat");

        app.MapEndpoints();
    }
}