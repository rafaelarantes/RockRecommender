namespace RockRecommender.Domain.Repositories;

public interface IRecommendationLogRepository
{
    Task<List<Guid>> GetShownSongIdsAsync(Guid userId);
    Task RegisterShownAsync(Guid userId, Guid songId);
}
