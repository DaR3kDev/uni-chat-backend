using Microsoft.Extensions.DependencyInjection;
using uni_chat_backend.Features.Contacts.GetContacts.Cache;
using uni_chat_backend.Features.Contacts.GetContacts.Interfaces;

namespace uni_chat_backend.Features.Contacts.GetContacts;

public static class GetContactsDependencyInjection
{
    public static void AddGetContactFeature(this IServiceCollection services)
    {
        services.AddScoped<IGetContactsCache, GetContactsCache>();
    }
}
