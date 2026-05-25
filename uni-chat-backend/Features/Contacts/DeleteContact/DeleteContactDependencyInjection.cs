using FluentValidation;
using uni_chat_backend.Features.Contacts.DeleteContact.Cache;
using uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;
using uni_chat_backend.Features.Contacts.DeleteContact.Services;
using uni_chat_backend.Features.Contacts.DeleteContact.Validators;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public static class DeleteContactDependencyInjection
{
    public static void AddDeleteContactFeature(this IServiceCollection services)
    {
        // =========================================================
        // SERVICES
        // =========================================================
        services.AddScoped<IDeleteContactService, DeleteContactService>();

        // =========================================================
        // CACHE
        // =========================================================
        services.AddScoped<IDeleteContactCache, DeleteContactCache>();

        // =========================================================
        // VALIDATORS
        // =========================================================
        services.AddScoped<IValidator<DeleteContactCommand>, DeleteContactValidator>();
    }
}
