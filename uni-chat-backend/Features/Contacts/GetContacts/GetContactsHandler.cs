using MediatR;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.GetContacts;

public class GetContactsHandler(
    IContactRepository contactRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetContactsQuery, List<ContactResponse>>
{
    public async Task<List<ContactResponse>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
            ?? throw new Exception("No autorizado");

        var contacts = await contactRepository.GetByOwnerPagedAsync(
            ownerUserId,
            request.Page,
            request.PageSize,
            request.Search
        );

        var userIds = contacts.Select(c => c.ContactUserId).ToList();

        var users = await userRepository.GetByIdsAsync(userIds);

        var usersDict = users.ToDictionary(u => u.Id);

        return [.. contacts.Select(contact =>
        {
            usersDict.TryGetValue(contact.ContactUserId, out var user);

            return new ContactResponse(
                contact.Id,
                contact.ContactUserId,
                user?.Username,
                user?.Phone,
                contact.Alias
            );
        })];
    }
}