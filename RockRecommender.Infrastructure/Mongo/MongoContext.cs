using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RockRecommender.Infrastructure.Mongo.Documents;

namespace RockRecommender.Infrastructure.Mongo;

public sealed class MongoContext
{
    public MongoContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.Database);

        Songs = database.GetCollection<SongDocument>("songs");
        Users = database.GetCollection<UserDocument>("users");
        Feedback = database.GetCollection<FeedbackDocument>("feedback");
        RecommendationLog = database.GetCollection<RecommendationLogDocument>("recommendationLog");
    }

    public IMongoCollection<SongDocument> Songs { get; }
    public IMongoCollection<UserDocument> Users { get; }
    public IMongoCollection<FeedbackDocument> Feedback { get; }
    public IMongoCollection<RecommendationLogDocument> RecommendationLog { get; }
}
