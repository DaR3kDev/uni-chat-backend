using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.Infrastructure.SignalR;

public abstract class BaseHub : Hub
{
    protected static void HandleException(Exception ex)
    {
        if (ex is AppException appEx)
            throw new HubException(appEx.Message);

        throw new HubException("Internal server error");
    }
}
