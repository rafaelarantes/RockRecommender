namespace RockRecommender.Training.RealFeedback;

public sealed record TrainingInteractions(List<Interaction> Interactions, bool IsReal);
