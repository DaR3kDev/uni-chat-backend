namespace uni_chat_backend.Features.Contacts.AddContact.Interfaces;

public interface IAddContactService
{
    Task<string> AddAsync(AddContactCommand request, CancellationToken cancellationToken);
}
