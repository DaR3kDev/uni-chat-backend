using JasperFx.CodeGeneration;
using uni_chat_backend.Features.Conversations.JoinConversation.Contracts;
using uni_chat_backend.Features.Messages.SendMessage;
using uni_chat_backend.Infrastructure.Settings;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class WolverineConfiguration
{
    public static void AddWolverineConfiguration(
        this IHostBuilder host,
        IConfiguration configuration)
    {
        var rabbitSettings =
            configuration.GetSection("RabbitMQ")
                .Get<RabbitMqSettings>();

        if (rabbitSettings is null ||
            string.IsNullOrWhiteSpace(rabbitSettings.ConnectionString))
        {
            throw new InvalidOperationException(
                "RabbitMQ ConnectionString is missing or empty.");
        }

        host.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(rabbitSettings.ConnectionString))
                .AutoProvision();

            options.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            // PUBLICADORES
            options.PublishMessage<SendMessageEvent>()
                .ToRabbitQueue("messages.send");

            options.PublishMessage<UserJoinedConversation>()
                .ToRabbitQueue("conversations.user-joined");

            // LISTENERS
            options.ListenToRabbitQueue("conversations.user-joined")
                .UseDurableInbox()
                .MaximumParallelMessages(5);

            // RETRIES
            options.Policies
                .OnException<Exception>()
                .RetryTimes(3);

            // DEAD LETTER
            options.Policies
                .OnException<Exception>()
                .MoveToErrorQueue();

            // TRANSACCIONES
            options.Policies.AutoApplyTransactions();

            // DURABLE LOCAL QUEUES
            options.Policies.UseDurableLocalQueues();

            // SERIALIZACIÓN
            options.UseSystemTextJsonForSerialization();
        });
    }
}
