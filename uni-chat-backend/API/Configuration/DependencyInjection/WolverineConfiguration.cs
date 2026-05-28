using JasperFx.CodeGeneration;
using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;
using uni_chat_backend.Features.Messages.SendMessage.Contracts;
using uni_chat_backend.Infrastructure.Settings;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class WolverineConfiguration
{
    public static void AddWolverineConfiguration(this IHostBuilder host, IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

        if (rabbitSettings is null || string.IsNullOrWhiteSpace(rabbitSettings.ConnectionString))
        {
            throw new InvalidOperationException("RabbitMQ ConnectionString is missing or empty.");
        }

        host.UseWolverine(options =>
        {
            // =========================================
            // RABBITMQ
            // =========================================

            options.UseRabbitMq(new Uri(rabbitSettings.ConnectionString)).AutoProvision()
                .DeclareExchange("messages.sent", exchange =>
                {
                    exchange.BindQueue("messages.sent.realtime");
                    exchange.BindQueue("messages.sent.cache");
                });

            // =========================================
            // CODE GENERATION
            // =========================================

            options.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            // =========================================
            // MESSAGE SENT
            // =========================================

            options.PublishMessage<MessageSent>()
                .ToRabbitExchange("messages.sent");

            options.ListenToRabbitQueue("messages.sent.realtime")
                .UseDurableInbox();

            options.ListenToRabbitQueue("messages.sent.cache")
                .UseDurableInbox();

            // =========================================
            // USER JOINED
            // =========================================

            options.PublishMessage<UserJoinedConversation>().ToRabbitExchange("conversations.user-joined");

            options.ListenToRabbitQueue("conversations.user-joined.queue").UseDurableInbox();

            // =========================================
            // HANDLER DISCOVERY
            // =========================================

            options.Discovery.IncludeAssembly(typeof(WolverineConfiguration).Assembly);

            // =========================================
            // ERROR HANDLING
            // =========================================

            options.Policies.OnException<Exception>().RetryTimes(3).Then.MoveToErrorQueue();

            options.Policies.UseDurableLocalQueues();

            // =========================================
            // SERIALIZATION
            // =========================================

            options.UseSystemTextJsonForSerialization();
        });
    }
}
