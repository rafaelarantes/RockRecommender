using RockRecommender.Domain.Common;

namespace RockRecommender.Domain.Entities;

public sealed class Song
{
    public Guid Id { get; }
    public string Title { get; }
    public string Band { get; }
    public string Genre { get; }

    private Song(Guid id, string title, string band, string genre)
    {
        Id = id;
        Title = title;
        Band = band;
        Genre = genre;
    }

    public static Result<Song> Create(Guid id, string title, string band, string genre)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<Song>.Invalid("A song title cannot be empty.");

        if (string.IsNullOrWhiteSpace(band))
            return Result<Song>.Invalid("A song band cannot be empty.");

        if (string.IsNullOrWhiteSpace(genre))
            return Result<Song>.Invalid("A song genre cannot be empty.");

        return Result<Song>.Success(new Song(id, title, band, genre));
    }

    public override bool Equals(object? obj) => obj is Song other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
