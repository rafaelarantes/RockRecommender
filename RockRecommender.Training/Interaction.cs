namespace RockRecommender.Training;

public sealed record Interaction(Guid UserId, Guid SongId, bool Liked);
