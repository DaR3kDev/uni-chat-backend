using MediatR;
using uni_chat_backend.Features.Messages.GetMessages.Contracts;
using uni_chat_backend.Features.Messages.GetMessages.Interfaces;

namespace uni_chat_backend.Features.Messages.GetMessages.Handlers;

public class GetMessagesHandler(IGetMessagesService service)
    : IRequestHandler<GetMessagesQuery, List<GetMessagesResult>>
{
    public async Task<List<GetMessagesResult>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        return await service.ExecuteAsync(request, cancellationToken);
    }
}
