namespace RockRecommender.Application.Dtos;

public sealed record FeedbackRequest(Guid SongId, bool Liked);
