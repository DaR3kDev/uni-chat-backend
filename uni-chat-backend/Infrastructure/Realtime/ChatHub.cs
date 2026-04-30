using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using MediatR;
using uni_chat_backend.Features.Messages.SendMessage;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;

namespace uni_chat_backend.Infrastructure.Realtime;

[Authorize]
public class ChatHub(
    IMediator mediator,
    IConversationRepository conversationRepository
) : Hub
{
    private readonly IMediator _mediator = mediator;
    private readonly IConversationRepository _conversationRepository = conversationRepository;

    // =========================
    // CONNECTED
    // =========================

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();

        await _conversationRepository.SetUserOnlineAsync(userId);

        await base.OnConnectedAsync();
    }

    // =========================
    // DISCONNECTED
    // =========================

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();

        await _conversationRepository.SetUserOfflineAsync(userId);

        await base.OnDisconnectedAsync(exception);
    }

    // =========================
    // JOIN
    // =========================

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();

        var conversation = await _conversationRepository.GetByIdAsync(conversationId)
            ?? throw new HubException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == userId && !p.IsBanned))
            throw new HubException("No perteneces a esta conversación");

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

        await Clients.Group(conversationId.ToString())
            .SendAsync("UserJoined", new { userId, conversationId });
    }

    // =========================
    // SEND MESSAGE (TIEMPO REAL)
    // =========================

    public async Task SendMessage(Guid conversationId, string content)
    {
        var senderId = GetUserId();

        // 1. Guardar en DB + lógica negocio
        var result = await _mediator.Send(new SendMessageCommand(
            conversationId,
            content
        ));

        // 2. Enviar en tiempo real
        await Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", result);
    }

    // =========================
    // ACK (CONFIRMACIÓN DE ENTREGA)
    // =========================

    public async Task MessageReceived(Guid messageId, Guid conversationId)
    {
        var userId = GetUserId();

        // aquí podrías actualizar estado en DB si quieres
        await Clients.Group(conversationId.ToString())
            .SendAsync("MessageDelivered", new
            {
                messageId,
                userId
            });
    }

    // =========================
    // HELPERS
    // =========================

    private Guid GetUserId()
    {
        var value = Context.User?
            .FindFirst(ClaimTypes.NameIdentifier)?
            .Value;

        return Guid.TryParse(value, out var id)
            ? id
            : throw new HubException("No autorizado");
    }
}