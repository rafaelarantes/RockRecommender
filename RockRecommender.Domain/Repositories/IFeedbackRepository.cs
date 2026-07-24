using RockRecommender.Domain.Entities;

namespace RockRecommender.Domain.Repositories;

public interface IFeedbackRepository
{
    Task<List<Feedback>> GetByUserAsync(Guid userId);
    Task AddAsync(Feedback feedback);
}
