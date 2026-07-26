using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeRecommendationLogRepository : IRecommendationLogRepository
{
    public List<RecommendationLogEntry> Entries { get; } = [];

    public Task<List<Guid>> GetShownSongIdsAsync(Guid userId) =>
        Task.FromResult(Entries.Where(entry => entry.UserId == userId).Select(entry => entry.SongId).ToList());

    public Task<Guid?> GetLastShownSongIdAsync(Guid userId)
    {
        var lastEntry = Entries.Where(entry => entry.UserId == userId).MaxBy(entry => entry.ShownAt);

        return Task.FromResult(lastEntry is null ? (Guid?)null : lastEntry.SongId);
    }

    public Task RegisterShownAsync(Guid userId, Guid songId)
    {
        Entries.Add(new RecommendationLogEntry(userId, songId, DateTime.UtcNow));
        return Task.CompletedTask;
    }
}
