using RockRecommender.Application.Recommendations;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeCollaborativeRecommender : ICollaborativeRecommender
{
    public bool IsAvailable { get; set; } = true;
    public Func<Guid, Guid, float>? ScoreFunc { get; set; }

    public float PredictScore(Guid userId, Guid songId) => ScoreFunc?.Invoke(userId, songId) ?? 0f;
}
