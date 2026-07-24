namespace RockRecommender.Training;

public sealed class TrainingOptions
{
    public const string SectionName = "Training";

    public string ModelPath { get; set; } = "";
    public int SyntheticUserCount { get; set; }
    public int EvaluationK { get; set; }
}
