using uni_chat_backend.API.Endpoints.Auth;
using uni_chat_backend.API.Endpoints.Contact;
using uni_chat_backend.API.Endpoints.Conversations;
using uni_chat_backend.API.Endpoints.Messages;

namespace uni_chat_backend.API.Extensions;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        // Authentication endpoints
        app.MapLoginEndpoint();
        app.MapRegisterEndpoint();
        app.MapMeEndpoint();

        // Contact endpoints
        app.MapAddContactEndpoint();
        app.MapGetContactsEndpoint();
        app.MapDeleteContactEndpoint();

        //Conversation endpoints
        app.MapGetOrCreateDirectConversationEndpoint();
        app.MapGetConversationsEndpoint();
        app.MapJoinConversationEndpoint();

        // Message endpoints
        app.MapMessagesEndpoints();
        app.MapGetMessagesEndpoint();
    }
}