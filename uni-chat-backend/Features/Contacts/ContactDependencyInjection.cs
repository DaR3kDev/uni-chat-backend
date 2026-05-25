using uni_chat_backend.Features.Contacts.AddContact;
using uni_chat_backend.Features.Contacts.DeleteContact;
using uni_chat_backend.Features.Contacts.GetContacts;

namespace uni_chat_backend.Features.Contacts;

public static class ContactDependencyInjection
{
    public static void AddContactFeatures(this IServiceCollection services)
    {
        services.AddAddContactFeature();
        services.AddDeleteContactFeature();
        services.AddGetContactFeature();
    }
}
