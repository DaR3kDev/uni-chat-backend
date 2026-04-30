namespace uni_chat_backend.Infrastructure.Persistence.Indexes.Initialization;

public class MongoIndexesInitializerService(MongoContext context) : IHostedService
{
    private readonly MongoContext _context = context;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await MongoIndexesInitializer.Initialize(_context.Database);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}