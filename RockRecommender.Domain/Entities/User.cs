using RockRecommender.Domain.Common;

namespace RockRecommender.Domain.Entities;

public sealed class User
{
    private readonly List<string> _likedBandIds;

    public Guid Id { get; }

    public IReadOnlyList<string> LikedBandIds => _likedBandIds;

    private User(Guid id, List<string> likedBandIds)
    {
        Id = id;
        _likedBandIds = likedBandIds;
    }

    public static Result<User> Create(Guid id, IEnumerable<string> likedBandIds)
    {
        var distinctBandIds = likedBandIds.Distinct().ToList();

        if (distinctBandIds.Any(string.IsNullOrWhiteSpace))
            return Result<User>.Invalid("A liked band cannot be empty.");

        return Result<User>.Success(new User(id, distinctBandIds));
    }

    public bool Likes(Song song) => _likedBandIds.Contains(song.Band);

    public override bool Equals(object? obj) => obj is User other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
