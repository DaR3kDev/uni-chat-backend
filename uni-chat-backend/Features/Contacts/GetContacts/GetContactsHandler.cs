using System.Text.Json;
using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.GetContacts;

public class GetContactsHandler(
    IContactRepository contactRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<GetContactsQuery, List<ContactResponse>>
{
    public async Task<List<ContactResponse>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId
                          ?? throw new UnauthorizedException("No autorizado");

        var db = redis.GetDatabase();

        var cacheKey =
            $"contacts:{ownerUserId}:{request.Page}:{request.PageSize}:{request.Search}";

        var cachedContacts = await db.StringGetAsync(cacheKey);

        if (cachedContacts.HasValue)
        {
            var json = cachedContacts.ToString();

            if (!string.IsNullOrWhiteSpace(json))
            {
                var cachedResponse =
                    JsonSerializer.Deserialize<List<ContactResponse>>(json);

                if (cachedResponse is not null)
                    return cachedResponse;
            }
        }

        var contacts = await contactRepository.GetByOwnerPagedAsync(
            ownerUserId,
            request.Page,
            request.PageSize,
            request.Search
        );

        var userIds = contacts
            .Select(x => x.ContactUserId)
            .Distinct()
            .ToList();

        var users = await userRepository.GetByIdsAsync(userIds);

        var usersMap = users.ToDictionary(x => x.Id);

        var response = contacts
            .Select(contact =>
            {
                usersMap.TryGetValue(contact.ContactUserId, out var user);

                return new ContactResponse(
                    contact.Id,
                    contact.ContactUserId,
                    user?.Username,
                    user?.Phone,
                    contact.Alias
                );
            })
            .ToList();

        await db.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromMinutes(5)
        );

        return response;
    }
}
