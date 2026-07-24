using RockRecommender.Domain.Entities;

namespace RockRecommender.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Likes_ReturnsTrue_WhenSongBandIsInLikedBandIds()
    {
        //Arrange
        var user = User.Create(Guid.NewGuid(), ["Metallica"]).Value!;
        var song = Song.Create(Guid.NewGuid(), "Master of Puppets", "Metallica", "Thrash Metal").Value!;

        //Act & Assert
        Assert.True(user.Likes(song));
    }

    [Fact]
    public void Likes_ReturnsFalse_WhenSongBandIsNotInLikedBandIds()
    {
        //Arrange
        var user = User.Create(Guid.NewGuid(), ["Metallica"]).Value!;
        var song = Song.Create(Guid.NewGuid(), "Paranoid", "Black Sabbath", "Heavy Metal").Value!;

        //Act & Assert
        Assert.False(user.Likes(song));
    }

    [Fact]
    public void Equals_ReturnsTrue_ForUsersWithTheSameId()
    {
        //Arrange
        var userId = Guid.NewGuid();

        //Act
        var firstUser = User.Create(userId, ["Metallica"]).Value!;
        var secondUser = User.Create(userId, ["Black Sabbath"]).Value!

        //Assert
        Assert.Equal(firstUser, secondUser);
    }

    [Fact]
    public void Create_ReturnsInvalid_WhenALikedBandIsBlank()
    {
        //Arrange & Act
        var result = User.Create(Guid.NewGuid(), [" "]);

        //Assert
        Assert.False(result.IsSuccess);
    }
}
