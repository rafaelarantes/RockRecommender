namespace RockRecommender.Training.Synthetic;

public sealed class SyntheticInteractionOptions
{
    public const string SectionName = "SyntheticInteractions";

    public int MinPreferredGenreCount { get; set; } = 1;
    public int MaxPreferredGenreCount { get; set; } = 3;
    public double PreferredGenreInteractionProbability { get; set; } = 0.9;
    public double OtherGenreInteractionProbability { get; set; } = 0.25;
    public double PreferredGenreLikeProbability { get; set; } = 0.85;
    public double OtherGenreLikeProbability { get; set; } = 0.15;
}
