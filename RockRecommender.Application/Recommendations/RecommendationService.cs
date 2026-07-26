using RockRecommender.Application.Common;
using RockRecommender.Application.Dtos;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;
using RockRecommender.Domain.ValueObjects;

namespace RockRecommender.Application.Recommendations;

public sealed class RecommendationService(
    ISongRepository songRepository,
    IUserRepository userRepository,
    IFeedbackRepository feedbackRepository,
    IRecommendationLogRepository recommendationLogRepository,
    ICollaborativeRecommender collaborativeRecommender)
{
    public async Task<Result<SongResponse>> GetNextSongAsync(Guid userId)
    {
        var userResult = await GetUserAsync(userId);

        if (!userResult.IsSuccess)
            return Result<SongResponse>.NotFound(userResult.Error!);

        var candidatesResult = await GetCandidateSongsAsync(userId);
        
        if (!candidatesResult.IsSuccess)
            return Result<SongResponse>.Conflict(candidatesResult.Error!);

        var feedbackHistory = await GetFeedbackHistoryAsync(userId);

        var songResult = feedbackHistory.HasAnyFeedback
            ? PickByCollaborativeModel(userId, candidatesResult.Value!)
            : Result<Song>.Success(PickByContent(userResult.Value!, candidatesResult.Value!));

        if (!songResult.IsSuccess)
            return Result<SongResponse>.Unavailable(songResult.Error!);

        await RegisterAsShownAsync(userId, songResult.Value!);

        return Result<SongResponse>.Success(ToResponse(songResult.Value!));
    }

    private async Task<Result<User>> GetUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);

        return user is null
            ? Result<User>.NotFound($"User '{userId}' was not found.")
            : Result<User>.Success(user);
    }

    private async Task<Result<List<Song>>> GetCandidateSongsAsync(Guid userId)
    {
        var allSongs = await songRepository.GetAllAsync();

        if (allSongs.Count == 0)
            return Result<List<Song>>.Conflict("The song catalog is empty.");

        var shownSongIds = await recommendationLogRepository.GetShownSongIdsAsync(userId);
        var lastShownSongId = await recommendationLogRepository.GetLastShownSongIdAsync(userId);
        var lastShownBand = allSongs.FirstOrDefault(song => song.Id == lastShownSongId)?.Band;

        var history = new RecommendationHistory(shownSongIds, lastShownBand);

        return Result<List<Song>>.Success(history.SelectUnseenOrFallback(allSongs));
    }

    private async Task<FeedbackHistory> GetFeedbackHistoryAsync(Guid userId)
    {
        var feedback = await feedbackRepository.GetByUserAsync(userId);

        return new FeedbackHistory(feedback);
    }

    private async Task RegisterAsShownAsync(Guid userId, Song song) =>
        await recommendationLogRepository.RegisterShownAsync(userId, song.Id);

    private static Song PickByContent(User user, List<Song> candidates)
    {
        var likedSongs = candidates.Where(user.Likes).ToList();
        var pool = likedSongs.Count > 0 ? likedSongs : candidates;

        return pool[Random.Shared.Next(pool.Count)];
    }

    private Result<Song> PickByCollaborativeModel(Guid userId, List<Song> candidates)
    {
        if (!collaborativeRecommender.IsAvailable)
        {
            return Result<Song>.Unavailable(
                "The collaborative recommendation model is not loaded yet. Train it with RockRecommender.Training and restart the API.");
        }

        var bestSong = candidates[0];
        var bestScore = collaborativeRecommender.PredictScore(userId, bestSong.Id);

        foreach (var candidate in candidates.Skip(1))
        {
            var score = collaborativeRecommender.PredictScore(userId, candidate.Id);

            if (score > bestScore)
            {
                bestScore = score;
                bestSong = candidate;
            }
        }

        return Result<Song>.Success(bestSong);
    }

    private static SongResponse ToResponse(Song song) =>
        new(song.Id, song.Title, song.Band, song.Genre);
}
