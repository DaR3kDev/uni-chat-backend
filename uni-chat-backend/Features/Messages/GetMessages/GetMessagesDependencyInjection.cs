using uni_chat_backend.Features.Messages.GetMessages.Cache;
using uni_chat_backend.Features.Messages.GetMessages.Interfaces;
using uni_chat_backend.Features.Messages.GetMessages.Services;

namespace uni_chat_backend.Features.Messages.GetMessages;

public static class GetMessagesDependencyInjection
{
    public static void AddGetMessages(this IServiceCollection services)
    {
        services.AddScoped<IGetMessagesCache, GetMessagesCache>();

        services.AddScoped<IGetMessagesService, GetMessagesService>();
    }
}
