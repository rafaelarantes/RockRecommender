using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RockRecommender.Infrastructure.Mongo.Documents;

public sealed class FeedbackDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public Guid SongId { get; set; }
    public bool Liked { get; set; }
    public DateTime CreatedAt { get; set; }
}
