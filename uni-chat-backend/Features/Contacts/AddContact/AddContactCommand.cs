using MediatR;

namespace uni_chat_backend.Features.Contacts.AddContact;

public record AddContactCommand(
    string Phone,
    string? Alias
) : IRequest<string>;

