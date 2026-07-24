namespace RockRecommender.Application.Dtos;

public sealed record SongResponse(Guid Id, string Title, string Band, string Genre);
