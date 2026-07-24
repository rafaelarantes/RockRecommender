using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.ValueObjects;

public sealed class RecommendationHistory
{
    private readonly HashSet<Guid> _shownSongIds;

    public RecommendationHistory(IEnumerable<Guid> shownSongIds) =>
        _shownSongIds = [.. shownSongIds];

    public List<Song> SelectUnseenOrFallback(IReadOnlyList<Song> catalog)
    {
        var unseenSongs = catalog.Where(song => !_shownSongIds.Contains(song.Id)).ToList();

        return unseenSongs.Count > 0 ? unseenSongs : catalog.ToList();
    }
}
