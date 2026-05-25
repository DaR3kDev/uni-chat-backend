using MediatR;
using uni_chat_backend.Features.Contacts.DeleteContact.Interfaces;

namespace uni_chat_backend.Features.Contacts.DeleteContact;

public class DeleteContactHandler(IDeleteContactService deleteContactService)
    : IRequestHandler<DeleteContactCommand, string>
{
    public async Task<string> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        return await deleteContactService.DeleteAsync(request, cancellationToken);
    }
}
