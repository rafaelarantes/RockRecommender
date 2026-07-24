using RockRecommender.Application.Common;
using RockRecommender.Application.Dtos;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;

namespace RockRecommender.Application.Users;

public sealed class UserService(IUserRepository userRepository)
{
    public async Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest request)
    {
        var userCreation = User.Create(Guid.NewGuid(), request.LikedBands);
        
        if (!userCreation.IsSuccess)
            return Result<UserResponse>.Invalid(userCreation.Error!);

        await userRepository.AddAsync(userCreation.Value!);

        return Result<UserResponse>.Success(ToResponse(userCreation.Value!));
    }

    private static UserResponse ToResponse(User user) =>
        new(user.Id, user.LikedBandIds);
}
