using uni_chat_backend.Features.Messages.SendMessage.Cache;
using uni_chat_backend.Features.Messages.SendMessage.Interfaces;
using uni_chat_backend.Features.Messages.SendMessage.Services;

namespace uni_chat_backend.Features.Messages.SendMessage;

public static class SendMessagesDependencyInjection
{
    public static void AddSendMessages(this IServiceCollection services)
    {
        services.AddScoped<ISendMessageCache, SendMessageCache>();

        services.AddScoped<ISendMessageService, SendMessageService>();
    }
}
