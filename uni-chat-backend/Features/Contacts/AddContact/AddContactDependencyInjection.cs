using FluentValidation;
using uni_chat_backend.Features.Contacts.AddContact.Cache;
using uni_chat_backend.Features.Contacts.AddContact.Interfaces;
using uni_chat_backend.Features.Contacts.AddContact.Services;
using uni_chat_backend.Features.Contacts.AddContact.Validators;

namespace uni_chat_backend.Features.Contacts.AddContact;

public static class AddContactDependencyInjection
{
    public static void AddAddContactFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<IAddContactService, AddContactService>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IContactCache, ContactCache>();

        // =========================================================
        // VALIDATORS
        // =========================================================
        services.AddScoped<IValidator<AddContactCommand>, AddContactValidator>();
    }
}
