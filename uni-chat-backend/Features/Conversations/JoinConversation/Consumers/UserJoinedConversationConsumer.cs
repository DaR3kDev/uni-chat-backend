using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;

namespace uni_chat_backend.Features.Conversations.JoinConversation.Consumers;

public class UserJoinedConversationConsumer(ILogger<UserJoinedConversationConsumer> logger)
{
    public Task Handle(UserJoinedConversation message)
    {
        logger.LogInformation("Usuario se unió a la conversación. ConversationId: {ConversationId}, UserId: {UserId}",
            message.ConversationId, message.UserId);

        return Task.CompletedTask;
    }
}
