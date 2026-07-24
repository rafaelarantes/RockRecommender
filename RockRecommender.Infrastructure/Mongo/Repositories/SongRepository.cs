using MongoDB.Driver;
using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo.Repositories;

public sealed class SongRepository(MongoContext context) : ISongRepository
{
    public async Task<List<Song>> GetAllAsync()
    {
        var documents = await context.Songs.Find(Builders<SongDocument>.Filter.Empty).ToListAsync();
        return [.. documents.Select(ToDomain)];
    }

    private static Song ToDomain(SongDocument document) =>
        Song.Create(document.Id, document.Title, document.Band, document.Genre).Value!;
}
