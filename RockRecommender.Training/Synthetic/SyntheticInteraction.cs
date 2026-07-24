namespace RockRecommender.Training.Synthetic;

public sealed record SyntheticInteraction(Guid UserId, Guid SongId, bool Liked);
