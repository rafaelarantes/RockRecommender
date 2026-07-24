using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.Repositories;

public interface ISongRepository
{
    Task<List<Song>> GetAllAsync();
}
