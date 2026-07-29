using Microsoft.Extensions.Options;
using Microsoft.ML;
using RockRecommender.Application.Recommendations;

namespace RockRecommender.Infrastructure.MachineLearning;

public sealed class CollaborativeRecommenderModel : ICollaborativeRecommender, IDisposable
{
    private readonly string _modelPath;
    private readonly Lock _predictionLock = new();
    private PredictionEngine<SongRatingSample, SongRatingPrediction>? _predictionEngine;
    private DateTime _loadedAtUtc;

    public CollaborativeRecommenderModel(IOptions<CollaborativeModelOptions> options)
    {
        _modelPath = options.Value.Path;
        LoadIfExists();
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

    public void ReloadIfChanged()
    {
        if (!File.Exists(_modelPath) || File.GetLastWriteTimeUtc(_modelPath) <= _loadedAtUtc)
            return;

        LoadIfExists();
    }

    private void LoadIfExists()
    {
        if (!File.Exists(_modelPath))
            return;

        var mlContext = new MLContext();

        using var stream = File.OpenRead(_modelPath);
        var model = mlContext.Model.Load(stream, out _);
        var predictionEngine = mlContext.Model.CreatePredictionEngine<SongRatingSample, SongRatingPrediction>(model);
        var loadedAtUtc = File.GetLastWriteTimeUtc(_modelPath);

        lock (_predictionLock)
        {
            _predictionEngine?.Dispose();
            _predictionEngine = predictionEngine;
            _loadedAtUtc = loadedAtUtc;
        }
    }

    public void Dispose() => _predictionEngine?.Dispose();
}
