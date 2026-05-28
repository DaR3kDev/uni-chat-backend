using uni_chat_backend.Features.Messages.GetMessages;
using uni_chat_backend.Features.Messages.SendMessage;

namespace uni_chat_backend.Features.Messages;

public static class MessagesDependencyInjection
{
    public static void AddMessagesFeatures(this IServiceCollection services)
    {
        services.AddGetMessages();
        services.AddSendMessages();
    }
}
