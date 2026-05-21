using uni_chat_backend.Features.Messages.SendMessage;
using uni_chat_backend.Infrastructure.Settings;
using Wolverine;
using Wolverine.RabbitMQ;

namespace uni_chat_backend.API.Configuration.DependencyInjection;

public static class WolverineConfiguration
{
    public static void AddWolverineConfiguration(this IHostBuilder host, IConfiguration configuration)
    {
        var rabbitSettings = configuration
                                 .GetSection("RabbitMQ")
                                 .Get<RabbitMQSettings>()
                             ?? throw new InvalidOperationException("RabbitMQ settings missing");

        host.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(rabbitSettings.ConnectionString))
                .AutoProvision();


            options.PublishMessage<SendMessageEvent>()
                .ToRabbitQueue("messages.send");


            options.ListenToRabbitQueue("messages.send");
        });
    }
}
