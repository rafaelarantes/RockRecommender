using RockRecommender.Infrastructure.MachineLearning;
using RockRecommender.Training.Synthetic;

namespace RockRecommender.Training.Ml;

internal static class SongRatingSampleMapper
{
    public static SongRatingSample ToSample(SyntheticInteraction interaction) => new()
    {
        UserId = interaction.UserId.ToString(),
        SongId = interaction.SongId.ToString(),
        Label = interaction.Liked ? 1f : 0f,
    };
}
