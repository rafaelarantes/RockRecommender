using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RockRecommender.Infrastructure.Mongo.Documents;

public sealed class UserDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public List<string> LikedBandIds { get; set; } = [];
}
