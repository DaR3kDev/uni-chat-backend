using uni_chat_backend.Features.Messages.SendMessage;
using Wolverine;
using Wolverine.RabbitMQ;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class WolverineConfiguration
{
    public static void AddWolverineConfiguration(this IHostBuilder host, IConfiguration configuration)
    {
        var rabbitConnection =
            configuration["RabbitMQ:ConnectionString"]
            ?? throw new InvalidOperationException("RabbitMQ connection string is missing");

        host.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(rabbitConnection))
                .AutoProvision();

            options.PublishMessage<SendMessageEvent>()
                .ToRabbitQueue("messages.send");

            options.ListenToRabbitQueue("messages.send");
        });
    }
}
