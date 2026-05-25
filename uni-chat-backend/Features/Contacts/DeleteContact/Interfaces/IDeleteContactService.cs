namespace uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;

public interface IDeleteContactService
{
    Task<string> DeleteAsync(DeleteContactCommand request, CancellationToken cancellationToken);
}
