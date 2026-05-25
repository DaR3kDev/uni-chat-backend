namespace uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;

public interface IDeleteContactCache
{
    Task IncrementContactsVersionAsync(Guid userId);
}
