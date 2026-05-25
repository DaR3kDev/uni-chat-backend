using MediatR;
using uni_chat_backend.Features.Contacts.AddContact.Interfaces;

namespace uni_chat_backend.Features.Contacts.AddContact;

public class AddContactHandler(IAddContactService addContactService) : IRequestHandler<AddContactCommand, string>
{
    public async Task<string> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        return await addContactService.AddAsync(request, cancellationToken);
    }
}
