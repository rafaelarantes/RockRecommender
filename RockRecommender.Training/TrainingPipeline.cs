using Microsoft.Extensions.Options;
using Microsoft.ML;
using RockRecommender.Domain.Entities;
using RockRecommender.Infrastructure.Catalog;
using RockRecommender.Training.Evaluation;
using RockRecommender.Training.Ml;
using RockRecommender.Training.Reporting;
using RockRecommender.Training.Synthetic;

namespace RockRecommender.Training;

public sealed class TrainingPipeline(IOptions<TrainingOptions> options, SyntheticInteractionGenerator interactionGenerator)
{
    public void Run()
    {
        var settings = options.Value;
        var mlContext = new MLContext(seed: 1);

        var songs = LoadCatalog();
        var interactions = GenerateInteractions(songs, settings.SyntheticUserCount);

        EvaluateModel(mlContext, songs, interactions, settings.EvaluationK);
        TrainAndSaveFinalModel(mlContext, interactions, settings.ModelPath);
    }

    private static List<Song> LoadCatalog()
    {
        var songs = RockCatalog.GetSongs();
        ConsoleReport.PrintCatalogSummary(songs);

        return songs;
    }

    private List<SyntheticInteraction> GenerateInteractions(List<Song> songs, int syntheticUserCount)
    {
        var interactions = interactionGenerator.Generate(songs, syntheticUserCount);
        ConsoleReport.PrintInteractionSummary(interactions);

        return interactions;
    }

    private static void EvaluateModel(MLContext mlContext, List<Song> songs, List<SyntheticInteraction> interactions, int k)
    {
        var result = LeaveOneOutEvaluator.Evaluate(mlContext, interactions, songs, k);
        ConsoleReport.PrintEvaluationResult(result, k);
    }

    private static void TrainAndSaveFinalModel(MLContext mlContext, List<SyntheticInteraction> interactions, string modelPath)
    {
        var samples = interactions.Select(SongRatingSampleMapper.ToSample);
        var trainedModel = RecommenderModelTrainer.Train(mlContext, samples);

        mlContext.Model.Save(trainedModel.Model, trainedModel.InputSchema, modelPath);
        ConsoleReport.PrintModelSaved(modelPath);
    }
}
