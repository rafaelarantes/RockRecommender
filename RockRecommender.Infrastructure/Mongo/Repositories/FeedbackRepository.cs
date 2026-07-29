using MongoDB.Driver;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo.Repositories;

public sealed class FeedbackRepository(MongoContext context) : IFeedbackRepository
{
    public async Task<List<Feedback>> GetByUserAsync(Guid userId)
    {
        var documents = await context.Feedback.Find(f => f.UserId == userId).ToListAsync();
        return [.. documents.Select(ToDomain)];
    }

    public async Task<List<Feedback>> GetAllAsync()
    {
        var documents = await context.Feedback.Find(FilterDefinition<FeedbackDocument>.Empty).ToListAsync();
        
        return [.. documents.Select(ToDomain)];
    }

    public async Task AddAsync(Feedback feedback)
    {
        await context.Feedback.InsertOneAsync(new FeedbackDocument
        {
            UserId = feedback.UserId,
            SongId = feedback.SongId,
            Liked = feedback.Liked,
            CreatedAt = feedback.CreatedAt,
        });
    }

    private static Feedback ToDomain(FeedbackDocument document) =>
        new(document.UserId, document.SongId, document.Liked, document.CreatedAt);
}
