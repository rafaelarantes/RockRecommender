namespace RockRecommender.Application.Dtos;

public sealed record UserResponse(Guid Id, IReadOnlyList<string> LikedBands);
