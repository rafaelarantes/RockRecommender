namespace RockRecommender.Domain.Entities;

public sealed record Feedback(Guid UserId, Guid SongId, bool Liked, DateTime CreatedAt);
