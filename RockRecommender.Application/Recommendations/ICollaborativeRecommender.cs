namespace RockRecommender.Application.Recommendations;

public interface ICollaborativeRecommender
{
    bool IsAvailable { get; }
    float PredictScore(Guid userId, Guid songId);
}
