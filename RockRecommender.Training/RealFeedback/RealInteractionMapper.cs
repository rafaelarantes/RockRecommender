using RockRecommender.Application.Dtos;

namespace RockRecommender.Training.RealFeedback;

public static class RealInteractionMapper
{
    public static List<Interaction> ToInteractions(List<FeedbackResponse> feedback) =>
        [.. feedback.Select(item => new Interaction(item.UserId, item.SongId, item.Liked))];
}
