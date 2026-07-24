using MongoDB.Driver;
using RockRecommender.Infrastructure.Catalog;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo;

public sealed class MongoCatalogSeeder(MongoContext context)
{
    public async Task SeedSongsIfEmptyAsync()
    {
        var songCount = await context.Songs.CountDocumentsAsync(Builders<SongDocument>.Filter.Empty);

        if (songCount > 0)
            return;

        var documents = RockCatalog.GetSongs().Select(song => new SongDocument
        {
            Id = song.Id,
            Title = song.Title,
            Band = song.Band,
            Genre = song.Genre,
        });

        await context.Songs.InsertManyAsync(documents);
    }
}
