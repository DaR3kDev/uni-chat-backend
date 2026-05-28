using uni_chat_backend.Features.Conversations.JoinConversation.Cache;
using uni_chat_backend.Features.Conversations.JoinConversation.Interfaces;
using uni_chat_backend.Features.Conversations.JoinConversation.Services;

namespace uni_chat_backend.Features.Conversations.JoinConversation;

public static class JoinConversationDependencyInjection
{
    public static void AddJoinConversation(this IServiceCollection services)
    {
        services.AddScoped<IJoinConversationCache, JoinConversationCache>();

        services.AddScoped<IJoinConversationService, JoinConversationService>();
    }
}

