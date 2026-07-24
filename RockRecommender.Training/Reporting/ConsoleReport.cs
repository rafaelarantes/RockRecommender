using RockRecommender.Domain.Entities;
using RockRecommender.Infrastructure.Catalog;
using RockRecommender.Training.Evaluation;
using RockRecommender.Training.Synthetic;

namespace RockRecommender.Training.Reporting;

public static class ConsoleReport
{
    public static void PrintCatalogSummary(List<Song> songs)
    {
        PrintSectionHeader("Catalog");
        Console.WriteLine($"{songs.Count} songs, {RockCatalog.Bands.Count} bands, {RockCatalog.Genres.Count} genres");

        foreach (var genre in RockCatalog.Genres)
        {
            var songCountInGenre = songs.Count(song => song.Genre == genre);
            Console.WriteLine($"  {genre,-20} {songCountInGenre} songs");
        }
    }

    public static void PrintInteractionSummary(List<SyntheticInteraction> interactions)
    {
        var likedCount = interactions.Count(interaction => interaction.Liked);
        var dislikedCount = interactions.Count - likedCount;
        var distinctUserCount = interactions.Select(interaction => interaction.UserId).Distinct().Count();

        PrintSectionHeader("Synthetic interactions");

        Console.WriteLine($"{interactions.Count} interactions from {distinctUserCount} synthetic users");
        Console.WriteLine($"  {likedCount} likes, {dislikedCount} dislikes");
    }

    public static void PrintEvaluationResult(LeaveOneOutResult result, int k)
    {
        PrintSectionHeader($"Leave-one-out evaluation (K={k})");

        Console.WriteLine($"Evaluated users: {result.EvaluatedUserCount}");
        Console.WriteLine($"  Precision@{k}:        {result.PrecisionAtK:P0}");
        Console.WriteLine($"  Recall@{k} (hit-rate): {result.RecallAtK:P0}");
        Console.WriteLine($"  NDCG@{k}:              {result.NdcgAtK:F2}");
    }

    public static void PrintModelSaved(string modelPath)
    {
        PrintSectionHeader("Final model");

        Console.WriteLine($"Saved trained model to {Path.GetFullPath(modelPath)}");
    }

    private static void PrintSectionHeader(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"== {title} ==");
    }
}
