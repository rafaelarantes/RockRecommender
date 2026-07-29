using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace RockRecommender.Infrastructure.MachineLearning;

public sealed class ModelReloadBackgroundService(CollaborativeRecommenderModel model, IOptions<CollaborativeModelOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.ReloadCheckInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
            model.ReloadIfChanged();
    }
}
