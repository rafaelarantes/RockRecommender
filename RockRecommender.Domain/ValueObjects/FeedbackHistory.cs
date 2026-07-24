using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.ValueObjects;

public sealed class FeedbackHistory(IReadOnlyList<Feedback> entries)
{
    public bool HasAnyFeedback => entries.Count > 0;
}
