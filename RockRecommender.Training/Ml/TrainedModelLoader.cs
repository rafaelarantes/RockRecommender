using Microsoft.ML;
using RockRecommender.Infrastructure.MachineLearning;

namespace RockRecommender.Training.Ml;

public static class TrainedModelLoader
{
    public static PredictionEngine<SongRatingSample, SongRatingPrediction>? TryLoad(MLContext mlContext, string modelPath)
    {
        if (!File.Exists(modelPath))
            return null;

        using var stream = File.OpenRead(modelPath);
        var model = mlContext.Model.Load(stream, out _);

        return mlContext.Model.CreatePredictionEngine<SongRatingSample, SongRatingPrediction>(model);
    }
}
