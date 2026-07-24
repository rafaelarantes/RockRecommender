using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task AddAsync(User user);
}
