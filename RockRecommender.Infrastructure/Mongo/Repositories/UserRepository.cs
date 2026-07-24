using MongoDB.Driver;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo.Repositories;

public sealed class UserRepository(MongoContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid userId)
    {
        var document = await context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        return document is null ? null : User.Create(document.Id, document.LikedBandIds).Value;
    }

    public async Task AddAsync(User user)
    {
        await context.Users.InsertOneAsync(new UserDocument
        {
            Id = user.Id,
            LikedBandIds = user.LikedBandIds.ToList(),
        });
    }
}
