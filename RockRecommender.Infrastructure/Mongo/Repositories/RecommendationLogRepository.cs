using MongoDB.Driver;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo.Repositories;

public sealed class RecommendationLogRepository(MongoContext context) : IRecommendationLogRepository
{
    public async Task<List<Guid>> GetShownSongIdsAsync(Guid userId)
    {
        var documents = await context.RecommendationLog.Find(entry => entry.UserId == userId).ToListAsync();

        return [.. documents.Select(entry => entry.SongId).Distinct()];
    }

    public async Task RegisterShownAsync(Guid userId, Guid songId)
    {
        await context.RecommendationLog.InsertOneAsync(new RecommendationLogDocument
        {
            UserId = userId,
            SongId = songId,
            ShownAt = DateTime.UtcNow,
        });
    }
}
