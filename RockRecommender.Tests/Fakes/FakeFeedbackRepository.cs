using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeFeedbackRepository : IFeedbackRepository
{
    public List<Feedback> Items { get; } = [];

    public Task<List<Feedback>> GetByUserAsync(Guid userId) =>
        Task.FromResult(Items.Where(item => item.UserId == userId).ToList());

    public Task<List<Feedback>> GetAllAsync() =>
        Task.FromResult(Items.ToList());

    public Task AddAsync(Feedback feedback)
    {
        Items.Add(feedback);
        return Task.CompletedTask;
    }
}
