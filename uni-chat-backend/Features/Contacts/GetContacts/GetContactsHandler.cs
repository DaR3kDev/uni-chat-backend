using MediatR;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Features.Contacts.GetContacts.Interfaces;
using uni_chat_backend.Features.Contacts.Shared;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Contacts.GetContacts;

public class GetContactsHandler(
    IContactRepository contactRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser,
    IGetContactsCache cache,
    ILogger<GetContactsHandler> logger) : IRequestHandler<GetContactsQuery, List<ContactResponse>>
{
    public async Task<List<ContactResponse>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var ownerUserId = currentUser.UserId ?? throw new UnauthorizedException("No autorizado");

        logger.LogInformation(
            "GetContacts iniciado. UserId: {UserId}, Page: {Page}, PageSize: {PageSize}, Search: {Search}", ownerUserId,
            request.Page, request.PageSize, request.Search);

        var cached = await cache.GetAsync(ownerUserId, request);

        if (cached is not null)
        {
            logger.LogInformation("GetContacts cache HIT. UserId: {UserId}", ownerUserId);

            return cached;
        }

        logger.LogInformation("GetContacts cache MISS. Consultando base de datos. UserId: {UserId}", ownerUserId);

        var contacts = await contactRepository.GetByOwnerPagedAsync(
            ownerUserId, request.Page, request.PageSize, request.Search);

        var userIds = contacts.Select(x => x.ContactUserId).Distinct().ToList();

        var users = await userRepository.GetByIdsAsync(userIds);
        var usersMap = users.ToDictionary(x => x.Id);

        var response = contacts.Select(contact =>
            {
                usersMap.TryGetValue(contact.ContactUserId, out var user);

                return new ContactResponse(contact.Id, contact.ContactUserId, user?.Username, user?.Phone,
                    contact.Alias);
            })
            .ToList();

        await cache.SetAsync(ownerUserId, request, response);

        logger.LogInformation("GetContacts finalizado. Resultados: {Count}, UserId: {UserId}", response.Count,
            ownerUserId);

        return response;
    }
}
