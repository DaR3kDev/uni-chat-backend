using System.Text.Json;
using MediatR;
using StackExchange.Redis;
using uni_chat_backend.Application.Common.Exceptions;
using uni_chat_backend.Domain.Entities;
using uni_chat_backend.Infrastructure.Repositories.Interfaces;
using uni_chat_backend.Infrastructure.Security;
using uni_chat_backend.Infrastructure.Security.Interfaces;

namespace uni_chat_backend.Features.Conversations.GetOrCreateDirect;

public class GetOrCreateConversationHandler(
    IConversationRepository conversationRepository,
    ICurrentUserService currentUser,
    IConnectionMultiplexer redis
) : IRequestHandler<GetOrCreateConversationCommand, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository = conversationRepository;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IConnectionMultiplexer _redis = redis;

    public async Task<ConversationDto> Handle(GetOrCreateConversationCommand request, CancellationToken ct)
    {
        var ownerUserId = _currentUser.UserId
                          ?? throw new UnauthorizedException("Usuario no autenticado");

        var db = _redis.GetDatabase();

        var cacheKey = $"conversation:direct:{ownerUserId}:{request.ContactUserId}";

        var cachedConversation = await db.StringGetAsync(cacheKey);

        if (cachedConversation.HasValue)
        {
            var cachedJson = cachedConversation.ToString();

            if (!string.IsNullOrWhiteSpace(cachedJson))
            {
                var cachedDto =
                    JsonSerializer.Deserialize<ConversationDto>(cachedJson);

                if (cachedDto is not null)
                    return cachedDto;
            }
        }

        var existingConversation =
            await _conversationRepository.GetDirectConversationAsync(
                ownerUserId,
                request.ContactUserId
            );

        if (existingConversation is not null)
        {
            var existingDto = new ConversationDto(
                existingConversation.Id,
                request.ContactUserId,
                existingConversation.CreatedAt,
                existingConversation.LastMessageAt
            );

            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(existingDto),
                TimeSpan.FromMinutes(10)
            );

            return existingDto;
        }

        var conversationId = Guid.NewGuid();

        var newConversation = new Conversation
        {
            Id = conversationId,
            IsGroup = false,
            CreatedAt = DateTime.UtcNow,

            Participants =
            [
                new ConversationParticipant
                {
                    UserId = ownerUserId
                },

                new ConversationParticipant
                {
                    UserId = request.ContactUserId
                }
            ],

            EncryptionKey = Convert.ToBase64String(
                E2EEncryptionService.GenerateKey()
            )
        };

        await _conversationRepository.CreateAsync(newConversation);

        var response = new ConversationDto(
            conversationId,
            request.ContactUserId,
            newConversation.CreatedAt,
            null
        );

        await db.StringSetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromMinutes(10)
        );

        await db.KeyDeleteAsync($"conversations:{ownerUserId}");

        return response;
    }
}
