namespace RockRecommender.Domain.Entities;

public sealed record RecommendationLogEntry(Guid UserId, Guid SongId, DateTime ShownAt);
