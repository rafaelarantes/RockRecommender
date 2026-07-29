namespace RockRecommender.Infrastructure.MachineLearning;

public sealed class CollaborativeModelOptions
{
    public const string SectionName = "Model";

    public string Path { get; set; } = "";
    public TimeSpan ReloadCheckInterval { get; set; }
}
