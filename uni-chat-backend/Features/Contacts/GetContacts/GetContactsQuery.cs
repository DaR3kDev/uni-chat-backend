using MediatR;

namespace uni_chat_backend.Features.Contacts.GetContacts;

public record GetContactsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null
) : IRequest<List<ContactResponse>>;