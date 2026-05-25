using MediatR;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public sealed record DeleteContactCommand(Guid ContactId) : IRequest<string>;
