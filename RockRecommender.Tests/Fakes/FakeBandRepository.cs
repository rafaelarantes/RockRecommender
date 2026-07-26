using RockRecommender.Domain.Repositories;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeBandRepository : IBandRepository
{
    public List<string> Bands { get; } = [];

    public Task<List<string>> GetAllAsync() => Task.FromResult(Bands.ToList());
}
