using uni_chat_backend.Features.Conversations.GetConversations;
using uni_chat_backend.Features.Conversations.GetOrCreateDirect;
using uni_chat_backend.Features.Conversations.JoinConversation;

namespace uni_chat_backend.Features.Conversations;

public static class ConversationsDependencyInjection
{
    public static void AddConversationsFeatures(this IServiceCollection services)
    {
        services.AddGetConversations();
        services.AddGetOrCreateConversation();
        services.AddJoinConversation();
    }
}
