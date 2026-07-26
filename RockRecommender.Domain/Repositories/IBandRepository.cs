namespace RockRecommender.Domain.Repositories;

public interface IBandRepository
{
    Task<List<string>> GetAllAsync();
}
