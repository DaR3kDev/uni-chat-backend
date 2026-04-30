using MongoDB.Driver;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Persistence.Collections;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Repositories;

public class ContactRepository(IMongoCollections mongoCollections) : IContactRepository
{
    private readonly IMongoCollection<Contact> _contacts = mongoCollections.Contacts;

    public Task CreateAsync(Contact contact) =>
        _contacts.InsertOneAsync(contact);

    public async Task<bool> ExistsAsync(Guid ownerUserId, Guid contactUserId)
    {
        var filter = Builders<Contact>.Filter.And(
            Builders<Contact>.Filter.Eq(x => x.OwnerUserId, ownerUserId),
            Builders<Contact>.Filter.Eq(x => x.ContactUserId, contactUserId)
        );

        return await _contacts.Find(filter).AnyAsync();
    }

    public async Task<List<Contact>> GetByOwnerPagedAsync(Guid ownerUserId, int page, int pageSize, string? search)
    {
        var filter = Builders<Contact>.Filter.Eq(x => x.OwnerUserId, ownerUserId);

        if (!string.IsNullOrEmpty(search))
        {
            filter = Builders<Contact>.Filter.And(
                filter,
                Builders<Contact>.Filter.Regex("Alias", new MongoDB.Bson.BsonRegularExpression(search, "i"))
            );
        }

        return await _contacts
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public Task<List<Contact>> GetByOwnerAsync(Guid ownerUserId) =>
        _contacts
            .Find(x => x.OwnerUserId == ownerUserId)
            .ToListAsync();

    public async Task<Contact?> GetByIdAsync(Guid contactId) =>
        await _contacts
            .Find(x => x.Id == contactId)
            .FirstOrDefaultAsync();

    public Task DeleteAsync(Guid contactId) =>
        _contacts.DeleteOneAsync(x => x.Id == contactId);
}