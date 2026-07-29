using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RockRecommender.Training;

public sealed class RetrainingBackgroundService(IServiceScopeFactory scopeFactory, IOptions<TrainingOptions> options, ILogger<RetrainingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.RetrainInterval);

        do await RunPipelineAsync();
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task RunPipelineAsync()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var pipeline = scope.ServiceProvider.GetRequiredService<TrainingPipeline>();

            await pipeline.RunAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Retraining run failed");
        }
    }
}
