using uni_chat_backend.Features.Contacts.Shared;

namespace uni_chat_backend.Features.Contacts.GetContacts.Interfaces;

public interface IGetContactsCache
{

    Task<List<ContactResponse>?> GetAsync(Guid ownerUserId, GetContactsQuery query);

    Task SetAsync(Guid ownerUserId, GetContactsQuery query, List<ContactResponse> response);
}
