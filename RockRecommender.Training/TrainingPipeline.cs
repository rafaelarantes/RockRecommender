using Microsoft.Extensions.Options;
using Microsoft.ML;
using RockRecommender.Domain.Entities;
using RockRecommender.Infrastructure.Catalog;
using RockRecommender.Infrastructure.MachineLearning;
using RockRecommender.Training.Evaluation;
using RockRecommender.Training.Ml;
using RockRecommender.Training.RealFeedback;
using RockRecommender.Training.Reporting;

namespace RockRecommender.Training;

public sealed class TrainingPipeline(IOptions<TrainingOptions> options, InteractionSourceSelector interactionSourceSelector)
{
    public async Task RunAsync()
    {
        var settings = options.Value;
        var mlContext = new MLContext(seed: 1);

        var songs = LoadCatalog();
        var trainingInteractions = await SelectInteractionsAsync(songs, settings.SyntheticUserCount);

        var split = LeaveOneOutEvaluator.Split(trainingInteractions.Interactions);
        var candidateResult = EvaluateCandidate(mlContext, split, songs, settings.EvaluationK);
        var activeResult = EvaluateActiveModel(mlContext, split, songs, settings.EvaluationK, settings.ModelPath);

        ConsoleReport.PrintEvaluationResult("candidate", candidateResult, settings.EvaluationK);

        if (activeResult is not null)
            ConsoleReport.PrintEvaluationResult("active", activeResult, settings.EvaluationK);

        var promoted = activeResult is null || candidateResult.IsBetterThan(activeResult);

        if (promoted)
            TrainAndPromoteFinalModel(mlContext, trainingInteractions.Interactions, settings.CandidateModelPath, settings.ModelPath);

        ConsoleReport.PrintPromotionDecision(promoted, settings.ModelPath);
    }

    private static List<Song> LoadCatalog()
    {
        var songs = RockCatalog.GetSongs();
        ConsoleReport.PrintCatalogSummary(songs);

        return songs;
    }

    private async Task<TrainingInteractions> SelectInteractionsAsync(List<Song> songs, int syntheticUserCount)
    {
        var trainingInteractions = await interactionSourceSelector.SelectAsync(songs, syntheticUserCount);
        ConsoleReport.PrintInteractionSummary(trainingInteractions.Interactions, trainingInteractions.IsReal ? "real feedback" : "synthetic");

        return trainingInteractions;
    }

    private static LeaveOneOutResult EvaluateCandidate(MLContext mlContext, HoldOutSplit split, List<Song> songs, int k)
    {
        var trainedModel = RecommenderModelTrainer.Train(mlContext, split.TrainingInteractions.Select(SongRatingSampleMapper.ToSample));
        using var predictionEngine = mlContext.Model.CreatePredictionEngine<SongRatingSample, SongRatingPrediction>(trainedModel.Model);

        return LeaveOneOutEvaluator.Evaluate(split, songs, predictionEngine, k);
    }

    private static LeaveOneOutResult? EvaluateActiveModel(MLContext mlContext, HoldOutSplit split, List<Song> songs, int k, string activeModelPath)
    {
        using var predictionEngine = TrainedModelLoader.TryLoad(mlContext, activeModelPath);

        return predictionEngine is null ? null : LeaveOneOutEvaluator.Evaluate(split, songs, predictionEngine, k);
    }

    private static void TrainAndPromoteFinalModel(MLContext mlContext, List<Interaction> interactions, string candidateModelPath, string activeModelPath)
    {
        var samples = interactions.Select(SongRatingSampleMapper.ToSample);
        var trainedModel = RecommenderModelTrainer.Train(mlContext, samples);

        mlContext.Model.Save(trainedModel.Model, trainedModel.InputSchema, candidateModelPath);
        File.Copy(candidateModelPath, activeModelPath, overwrite: true);
    }
}
