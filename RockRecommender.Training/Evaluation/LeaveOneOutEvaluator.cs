using Microsoft.ML;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Evaluation;
using RockRecommender.Infrastructure.MachineLearning;
using RockRecommender.Training.Ml;

namespace RockRecommender.Training.Evaluation;

public static class LeaveOneOutEvaluator
{
    public static HoldOutSplit Split(List<Interaction> interactions, int seed = 123)
    {
        var random = new Random(seed);
        var interactionsByUser = interactions.GroupBy(interaction => interaction.UserId).ToDictionary(g => g.Key, g => g.ToList());

        var heldOutSongIdByUser = new Dictionary<Guid, Guid>();
        var trainingInteractions = new List<Interaction>();

        foreach (var (userId, userInteractions) in interactionsByUser)
        {
            var likedSongIds = userInteractions.Where(interaction => interaction.Liked).Select(interaction => interaction.SongId).ToList();

            if (likedSongIds.Count == 0)
            {
                trainingInteractions.AddRange(userInteractions);
                continue;
            }

            var heldOutSongId = likedSongIds[random.Next(likedSongIds.Count)];

            heldOutSongIdByUser[userId] = heldOutSongId;
            trainingInteractions.AddRange(userInteractions.Where(interaction => interaction.SongId != heldOutSongId));
        }

        return new HoldOutSplit(heldOutSongIdByUser, trainingInteractions);
    }

    public static LeaveOneOutResult Evaluate(HoldOutSplit split, List<Song> songs, PredictionEngine<SongRatingSample, SongRatingPrediction> predictionEngine, int k)
    {
        var seenSongIdsByUser = GroupSeenSongIdsByUser(split.TrainingInteractions);

        return AggregateMetrics(split.HeldOutSongIdByUser, seenSongIdsByUser, songs, predictionEngine, k);
    }

    private static Dictionary<Guid, HashSet<Guid>> GroupSeenSongIdsByUser(List<Interaction> trainingInteractions) =>
        trainingInteractions
            .GroupBy(interaction => interaction.UserId)
            .ToDictionary(g => g.Key, g => g.Select(interaction => interaction.SongId).ToHashSet());

    private static LeaveOneOutResult AggregateMetrics(
        Dictionary<Guid, Guid> heldOutSongIdByUser,
        Dictionary<Guid, HashSet<Guid>> seenSongIdsByUser,
        List<Song> songs,
        PredictionEngine<SongRatingSample, SongRatingPrediction> predictionEngine,
        int k)
    {
        var precisionSum = 0d;
        var recallSum = 0d;
        var ndcgSum = 0d;
        var evaluatedUserCount = 0;

        foreach (var (userId, heldOutSongId) in heldOutSongIdByUser)
        {
            var rankedSongIds = RankCandidatesForUser(userId, heldOutSongId, seenSongIdsByUser, songs, predictionEngine);

            if (rankedSongIds is null)
            {
                continue;
            }

            var relevantSongIds = new HashSet<Guid> { heldOutSongId };

            precisionSum += RankingMetrics.PrecisionAtK(rankedSongIds, relevantSongIds, k);
            recallSum += RankingMetrics.RecallAtK(rankedSongIds, relevantSongIds, k);
            ndcgSum += RankingMetrics.NdcgAtK(rankedSongIds, relevantSongIds, k);
            evaluatedUserCount++;
        }

        return evaluatedUserCount == 0
            ? new LeaveOneOutResult(0, 0, 0, 0)
            : new LeaveOneOutResult(precisionSum / evaluatedUserCount, recallSum / evaluatedUserCount, ndcgSum / evaluatedUserCount, evaluatedUserCount);
    }

    private static List<Guid>? RankCandidatesForUser(
        Guid userId,
        Guid heldOutSongId,
        Dictionary<Guid, HashSet<Guid>> seenSongIdsByUser,
        List<Song> songs,
        PredictionEngine<SongRatingSample, SongRatingPrediction> predictionEngine)
    {
        var seenSongIds = seenSongIdsByUser.GetValueOrDefault(userId) ?? [];
        var candidateSongs = songs.Where(song => !seenSongIds.Contains(song.Id)).ToList();

        if (!candidateSongs.Any(song => song.Id == heldOutSongId))
            return null;

        var scoredSongs = candidateSongs
            .Select(song => new ScoredSong(song.Id, predictionEngine.Predict(new SongRatingSample { UserId = userId.ToString(), SongId = song.Id.ToString() }).Score))
            .ToList();

        var heldOutScore = scoredSongs.Single(scoredSong => scoredSong.SongId == heldOutSongId).Score;

        if (float.IsNaN(heldOutScore))
            return null;

        return [.. scoredSongs
            .Where(scoredSong => !float.IsNaN(scoredSong.Score))
            .OrderByDescending(scoredSong => scoredSong.Score)
            .Select(scoredSong => scoredSong.SongId)];
    }
}
