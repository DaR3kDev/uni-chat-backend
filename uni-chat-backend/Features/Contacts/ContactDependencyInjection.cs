using uni_chat_backend.Features.Contacts.AddContact;

namespace uni_chat_backend.Features.Contacts;

public static class ContactDependecyInjection
{
    public static void AddContactFeatures(this IServiceCollection services)
    {
        services.AddAddContactFeature();
       
    }
}
