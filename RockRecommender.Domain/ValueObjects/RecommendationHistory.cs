using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.ValueObjects;

public sealed class RecommendationHistory
{
    private readonly HashSet<Guid> _shownSongIds;
    private readonly string? _lastShownBand;

    public RecommendationHistory(IEnumerable<Guid> shownSongIds, string? lastShownBand = null)
    {
        _shownSongIds = [.. shownSongIds];
        _lastShownBand = lastShownBand;
    }

    public List<Song> SelectUnseenOrFallback(IReadOnlyList<Song> catalog)
    {
        var unseenSongs = catalog.Where(song => !_shownSongIds.Contains(song.Id)).ToList();
        var candidates = unseenSongs.Count > 0 ? unseenSongs : catalog.ToList();

        if (_lastShownBand is null)
            return candidates;

        var differentBand = candidates.Where(song => song.Band != _lastShownBand).ToList();

        return differentBand.Count > 0 ? differentBand : candidates;
    }
}
