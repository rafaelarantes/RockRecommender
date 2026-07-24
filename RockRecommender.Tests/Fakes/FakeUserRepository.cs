using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    public Dictionary<Guid, User> UsersById { get; } = [];

    public Task<User?> GetByIdAsync(Guid userId) => Task.FromResult(UsersById.GetValueOrDefault(userId));

    public Task AddAsync(User user)
    {
        UsersById[user.Id] = user;
        return Task.CompletedTask;
    }
}
