using RockRecommender.Infrastructure.Catalog;

namespace RockRecommender.Tests.Infrastructure;

public class RockCatalogTests
{
    [Fact]
    public void GetSongs_ProducesNoDuplicateIds()
    {
        // Arrange
        var songs = RockCatalog.GetSongs();

        // Act
        var distinctIdCount = songs.Select(song => song.Id).Distinct().Count();

        // Assert
        Assert.Equal(songs.Count, distinctIdCount);
    }

    [Fact]
    public void GetSongs_ProducesTheSameIds_AcrossMultipleCalls()
    {
        // Arrange & Act
        var firstCallIds = RockCatalog.GetSongs().Select(song => song.Id).ToList();
        var secondCallIds = RockCatalog.GetSongs().Select(song => song.Id).ToList();

        //Assert
        Assert.Equal(firstCallIds, secondCallIds);
    }

    [Fact]
    public void GetSongs_ProducesNonEmptyIds()
    {
        // Arrange & Act
        var songs = RockCatalog.GetSongs();

        // Assert
        Assert.All(songs, song => Assert.NotEqual(Guid.Empty, song.Id));
    }
}
