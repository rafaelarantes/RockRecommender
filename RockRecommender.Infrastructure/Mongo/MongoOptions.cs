namespace RockRecommender.Infrastructure.Mongo;

public sealed class MongoOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = "";
    public string Database { get; set; } = "";
}
