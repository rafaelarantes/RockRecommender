using Microsoft.Extensions.Options;
using Microsoft.ML;
using RockRecommender.Application.Recommendations;

namespace RockRecommender.Infrastructure.MachineLearning;

public sealed class CollaborativeRecommenderModel : ICollaborativeRecommender, IDisposable
{
    private readonly PredictionEngine<SongRatingSample, SongRatingPrediction>? _predictionEngine;
    private readonly Lock _predictionLock = new();

    public CollaborativeRecommenderModel(IOptions<CollaborativeModelOptions> options)
    {
        var modelPath = options.Value.Path;

        if (!File.Exists(modelPath))
            return;

        var mlContext = new MLContext();

        using var stream = File.OpenRead(modelPath);
        
        var model = mlContext.Model.Load(stream, out _);
        
        _predictionEngine = mlContext.Model.CreatePredictionEngine<SongRatingSample, SongRatingPrediction>(model);
    }

    public bool IsAvailable => _predictionEngine is not null;

    public float PredictScore(Guid userId, Guid songId)
    {
        lock (_predictionLock)
        {
            var prediction = _predictionEngine!.Predict(new SongRatingSample { UserId = userId.ToString(), SongId = songId.ToString() });
            return prediction.Score;
        }
    }

    public void Dispose() => _predictionEngine?.Dispose();
}
