using RockRecommender.Training.Synthetic;

namespace RockRecommender.Training.Evaluation;

internal sealed record HoldOutSplit(Dictionary<Guid, Guid> HeldOutSongIdByUser, List<SyntheticInteraction> TrainingInteractions);
