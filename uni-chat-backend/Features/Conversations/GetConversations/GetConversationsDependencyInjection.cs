using uni_chat_backend.Features.Conversations.GetConversations.Cache;
using uni_chat_backend.Features.Conversations.GetConversations.Interfaces;
using uni_chat_backend.Features.Conversations.GetConversations.Services;

namespace uni_chat_backend.Features.Conversations.GetConversations;

public static class GetConversationsDependencyInjection
{
    public static void AddGetConversations(this IServiceCollection services)
    {
        services.AddScoped<IGetConversationsCache, GetConversationsCache>();

        services.AddScoped<IGetConversationsService, GetConversationsService>();
    }
}
