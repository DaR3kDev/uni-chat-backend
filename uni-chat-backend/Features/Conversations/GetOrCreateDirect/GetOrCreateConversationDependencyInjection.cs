using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Cache;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Interfaces;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect.Services;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect;

public static class GetOrCreateConversationDependencyInjection
{
    public static void AddGetOrCreateConversation(this IServiceCollection services)
    {
        services.AddScoped<IGetOrCreateConversationCache, GetOrCreateConversationCache>();

        services.AddScoped<IGetOrCreateConversationService, GetOrCreateConversationService>();
    }
}
