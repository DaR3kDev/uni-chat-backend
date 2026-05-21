using System.Text.Json;
using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Enums;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Messages.GetMessages;

public class GetMessagesHandler(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<GetMessagesQuery, List<GetMessagesResult>>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IMessageRepository _messageRepository = messageRepository;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<List<GetMessagesResult>> Handle(
        GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
                     ?? throw new UnauthorizedException("No autenticado");

        var db = _redis.GetDatabase();

        var cacheKey = $"messages:{request.ConversationId}";

        var cachedMessages = await db.StringGetAsync(cacheKey);

        if (cachedMessages.HasValue)
        {
            var cachedJson = cachedMessages.ToString();

            if (!string.IsNullOrWhiteSpace(cachedJson))
            {
                var messagesFromCache =
                    JsonSerializer.Deserialize<List<GetMessagesResult>>(cachedJson);

                if (messagesFromCache is not null)
                    return messagesFromCache;
            }
        }

        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId)
                           ?? throw new NotFoundException("Conversación no existe");

        if (!conversation.Participants.Any(p => p.UserId == userId))
            throw new ForbiddenException("No perteneces a esta conversación");

        var messages = await _messageRepository.GetByConversationIdAsync(request.ConversationId);

        var key = await _conversationRepository.GetEncryptionKeyAsync(request.ConversationId);

        var aesKey = Convert.FromBase64String(key);

        var response = messages.Select(m => new GetMessagesResult(
            m.Id,
            m.ConversationId,
            m.SenderId,
            m.Type == MessageType.TEXT && m.Content != null
                ? E2EEncryptionService.Decrypt(
                    m.Content,
                    aesKey
                )
                : null,
            m.FileUrl,
            m.FileName,
            m.Type,
            m.CreatedAt
        )).ToList();

        await db.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromMinutes(5)
        );

        return response;
    }
}
