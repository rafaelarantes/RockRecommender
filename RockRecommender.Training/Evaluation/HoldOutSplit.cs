namespace RockRecommender.Training.Evaluation;

public sealed record HoldOutSplit(Dictionary<Guid, Guid> HeldOutSongIdByUser, List<Interaction> TrainingInteractions);
