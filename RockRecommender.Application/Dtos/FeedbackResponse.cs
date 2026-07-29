namespace RockRecommender.Application.Dtos;

public sealed record FeedbackResponse(Guid UserId, Guid SongId, bool Liked);
