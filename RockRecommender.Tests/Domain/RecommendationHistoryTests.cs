using RockRecommender.Domain.Entities;
using RockRecommender.Domain.ValueObjects;

namespace RockRecommender.Tests.Domain;

public class RecommendationHistoryTests
{
    [Fact]
    public void SelectUnseenOrFallback_ExcludesAlreadyShownSongs()
    {

        // Arrange
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var catalog = new List<Song>
        {
            CreateSong(song1Id, "Song One", "Band A", "Classic Rock"),
            CreateSong(song2Id, "Song Two", "Band B", "Punk Rock"),
        };
        var history = new RecommendationHistory([song1Id]);

        // Act
        var unseen = history.SelectUnseenOrFallback(catalog);

        // Assert
        Assert.Single(unseen);
        Assert.Equal(song2Id, unseen[0].Id);
    }

    [Fact]
    public void SelectUnseenOrFallback_ReturnsWholeCatalog_WhenEverySongWasAlreadyShown()
    {
        // Arrange
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var catalog = new List<Song>
        {
            CreateSong(song1Id, "Song One", "Band A", "Classic Rock"),
            CreateSong(song2Id, "Song Two", "Band B", "Punk Rock"),
        };
        var history = new RecommendationHistory([song1Id, song2Id]);

        // Act
        var unseen = history.SelectUnseenOrFallback(catalog);

        // Assert
        Assert.Equal(2, unseen.Count);
    }

    [Fact]
    public void SelectUnseenOrFallback_ExcludesTheLastShownBand()
    {
        // Arrange
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var song3Id = Guid.NewGuid();
        var catalog = new List<Song>
        {
            CreateSong(song1Id, "Song One", "Band A", "Classic Rock"),
            CreateSong(song2Id, "Song Two", "Band A", "Classic Rock"),
            CreateSong(song3Id, "Song Three", "Band B", "Punk Rock"),
        };
        var history = new RecommendationHistory([], "Band A");

        // Act
        var unseen = history.SelectUnseenOrFallback(catalog);

        // Assert
        Assert.Single(unseen);
        Assert.Equal(song3Id, unseen[0].Id);
    }

    [Fact]
    public void SelectUnseenOrFallback_FallsBackToEveryBand_WhenOnlyTheLastShownBandIsLeft()
    {
        // Arrange
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var catalog = new List<Song>
        {
            CreateSong(song1Id, "Song One", "Band A", "Classic Rock"),
            CreateSong(song2Id, "Song Two", "Band A", "Classic Rock"),
        };
        var history = new RecommendationHistory([], "Band A");

        // Act
        var unseen = history.SelectUnseenOrFallback(catalog);

        // Assert
        Assert.Equal(2, unseen.Count);
    }

    private static Song CreateSong(Guid id, string title, string band, string genre) =>
        Song.Create(id, title, band, genre).Value!;
}
