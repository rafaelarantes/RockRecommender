namespace RockRecommender.Infrastructure.Catalog;

internal sealed class BandDocument
{
    public string Name { get; set; } = "";
    public string Genre { get; set; } = "";
    public List<string> Songs { get; set; } = [];
}
