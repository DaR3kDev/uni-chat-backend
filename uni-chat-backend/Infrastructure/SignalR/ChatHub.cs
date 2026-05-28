using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;
using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using Wolverine;

namespace uni_chat_backend.Infrastructure.SignalR;

[Authorize]
public class ChatHub(
    IMediator mediator,
    IConversationRepository conversationRepository,
    IMessageRepository messageRepository,
    IMessageBus bus) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserIdOrThrow();
        await conversationRepository.SetUserOnlineAsync(userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdOrThrow();
        await conversationRepository.SetUserOfflineAsync(userId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserIdOrThrow();

        var conversation = await conversationRepository.GetByIdAsync(conversationId) ??
                           throw new HubException("Conversación no existe");

        var isParticipant = conversation.Participants.Any(p => p.UserId == userId && !p.IsBanned);

        if (!isParticipant) throw new HubException("No perteneces a esta conversación");

        var groupName = conversationId.ToString();

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await bus.PublishAsync(new UserJoinedConversation(conversationId, userId, DateTime.UtcNow));

        await Clients.Caller.SendAsync("JoinedConversation", new { conversationId, success = true });
    }

    public async Task SendMessage(Guid conversationId, string? content, string? fileUrl, string? fileName,
        MessageType? type)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
            throw new HubException("El mensaje está vacío");

        var command = new SendMessageCommand(conversationId, content, fileUrl, fileName, type ?? MessageType.TEXT);

        await mediator.Send(command);
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task TypingStarted(Guid conversationId)
    {
        var userId = GetUserIdOrThrow();

        await Clients.Group(conversationId.ToString())
            .SendAsync("UserTyping", new { conversationId, userId, isTyping = true });
    }

    public async Task TypingStopped(Guid conversationId)
    {
        var userId = GetUserIdOrThrow();

        await Clients.Group(conversationId.ToString())
            .SendAsync("UserTyping", new { conversationId, userId, isTyping = false });
    }

    public async Task MessageDelivered(Guid messageId, Guid conversationId)
    {
        var userId = GetUserIdOrThrow();
        await messageRepository.UpdateStatusAsync(messageId, MessageStatus.DELIVERED);

        await Clients.Group(conversationId.ToString()).SendAsync("MessageDelivered", new { messageId, userId });
    }

    public async Task MessageRead(Guid messageId, Guid conversationId)
    {
        var userId = GetUserIdOrThrow();
        await messageRepository.UpdateStatusAsync(messageId, MessageStatus.READ);

        await Clients.Group(conversationId.ToString()).SendAsync("MessageRead", new { messageId, userId });
    }

    private Guid GetUserIdOrThrow()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     Context.User?.FindFirst("sub")?.Value ?? Context.User?.FindFirst("nameid")?.Value;

        if (string.IsNullOrWhiteSpace(userId)) throw new HubException("Usuario no autenticado");

        return Guid.TryParse(userId, out var id) ? id : throw new HubException("UserId inválido en token");
    }
}
