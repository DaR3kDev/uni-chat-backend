using uni_chat_backend.Domain.Entities;

namespace uni_chat_backend.Infrastructure.Repositories.Interfaces;

public interface IContactRepository
{
    Task CreateAsync(Contact contact);

    Task<bool> ExistsAsync(Guid ownerUserId, Guid contactUserId);

    Task<List<Contact>> GetByOwnerAsync(Guid ownerUserId);

    Task<Contact?> GetByIdAsync(Guid contactId);

    Task DeleteAsync(Guid contactId);

    Task<List<Contact>> GetByOwnerPagedAsync(Guid ownerUserId,int page,int pageSize,string? search);
}

