using RockRecommender.Domain.Entities;
using RockRecommender.Domain.ValueObjects;

namespace RockRecommender.Tests.Domain;

public class FeedbackHistoryTests
{
    [Fact]
    public void HasAnyFeedback_ReturnsFalse_WhenThereIsNoFeedback()
    {
        //Arrange
        var history = new FeedbackHistory([]);

        //Act & Assert
        Assert.False(history.HasAnyFeedback);
    }

    [Fact]
    public void HasAnyFeedback_ReturnsTrue_WhenThereIsAtLeastOneFeedback()
    {
        //Arrange
        var history = new FeedbackHistory([new Feedback(Guid.NewGuid(), Guid.NewGuid(), true, DateTime.UtcNow)]);

        //Act & Assert
        Assert.True(history.HasAnyFeedback);
    }
}
