namespace RockRecommender.Infrastructure.MachineLearning;

public sealed class SongRatingSample
{
    public string UserId { get; set; } = "";
    public string SongId { get; set; } = "";
    public float Label { get; set; }
}
