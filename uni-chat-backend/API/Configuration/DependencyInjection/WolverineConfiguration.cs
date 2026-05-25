using uni_chat_backend.Features.Messages.SendMessage;
using Wolverine;
using Wolverine.RabbitMQ;
using uni_chat_backend.Infrastructure.Settings;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class WolverineConfiguration
{
    public static void AddWolverineConfiguration(this IHostBuilder host, IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

        if (rabbitSettings is null || string.IsNullOrWhiteSpace(rabbitSettings.ConnectionString))
            throw new InvalidOperationException(
                "RabbitMQ ConnectionString is missing or empty. Check Render env vars (RabbitMQ__ConnectionString).");

        host.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(rabbitSettings.ConnectionString)).AutoProvision();

            // SOLO routing (NO listeners manuales)
            options.PublishMessage<SendMessageEvent>().ToRabbitQueue("messages.send");
        });
    }
}
