using RockRecommender.Domain.Repositories;

namespace RockRecommender.Infrastructure.Catalog;

public sealed class BandRepository : IBandRepository
{
    public Task<List<string>> GetAllAsync() => Task.FromResult(RockCatalog.Bands.ToList());
}
