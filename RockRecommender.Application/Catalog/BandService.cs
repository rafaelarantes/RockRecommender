using RockRecommender.Domain.Repositories;

namespace RockRecommender.Application.Catalog;

public sealed class BandService(IBandRepository bandRepository)
{
    public Task<List<string>> GetAllAsync() => bandRepository.GetAllAsync();
}
