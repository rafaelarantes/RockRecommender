namespace RockRecommender.Infrastructure.Catalog;

internal sealed class RockCatalogDocument
{
    public List<string> Genres { get; set; } = [];
    public List<BandDocument> Bands { get; set; } = [];
}
