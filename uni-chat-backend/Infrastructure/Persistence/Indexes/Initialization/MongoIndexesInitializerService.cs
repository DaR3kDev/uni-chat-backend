namespace uni_chat_backend.Infrastructure.Persistence.Indexes.Initialization;

public class MongoIndexesInitializerService(MongoContext context) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await MongoIndexesInitializer.Initialize(context.Database);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
