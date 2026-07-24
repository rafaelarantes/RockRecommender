using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RockRecommender.Infrastructure.Mongo.Documents;

public sealed class SongDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Band { get; set; } = "";
    public string Genre { get; set; } = "";
}
