using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Application.Common.Exceptions;

namespace uni_chat_backend.API.Endpoints.Hubs;

public abstract class BaseHub : Hub
{
    protected static void HandleException(Exception ex)
    {
        if (ex is AppException appEx)
            throw new HubException(appEx.Message);

        // errores inesperados
        throw new HubException("Internal server error");
    }
}