using System.ComponentModel.DataAnnotations;

namespace RockRecommender.Application.Dtos;

public sealed record CreateUserRequest([Required, MinLength(1)] List<string> LikedBands);
