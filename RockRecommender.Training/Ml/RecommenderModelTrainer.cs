using Microsoft.ML;
using Microsoft.ML.Trainers;
using RockRecommender.Infrastructure.MachineLearning;

namespace RockRecommender.Training.Ml;

public static class RecommenderModelTrainer
{
    public static TrainedModel Train(MLContext mlContext, IEnumerable<SongRatingSample> samples)
    {
        var trainingData = mlContext.Data.LoadFromEnumerable(samples);

        var pipeline = mlContext.Transforms.Conversion
            .MapValueToKey("UserIdEncoded", nameof(SongRatingSample.UserId))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("SongIdEncoded", nameof(SongRatingSample.SongId)));

        var trainerOptions = new MatrixFactorizationTrainer.Options
        {
            MatrixColumnIndexColumnName = "UserIdEncoded",
            MatrixRowIndexColumnName = "SongIdEncoded",
            LabelColumnName = nameof(SongRatingSample.Label),
            ApproximationRank = 32,
            NumberOfIterations = 20,
            NumberOfThreads = 1,
        };

        var trainingPipeline = pipeline.Append(mlContext.Recommendation().Trainers.MatrixFactorization(trainerOptions));

        var model = trainingPipeline.Fit(trainingData);

        return new TrainedModel(model, trainingData.Schema);
    }
}
