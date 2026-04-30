using MediatR;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public record DeleteContactCommand(
    Guid ContactId
) : IRequest<string>;