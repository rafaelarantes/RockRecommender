using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RockRecommender.Domain.Entities;

namespace RockRecommender.Infrastructure.Catalog;

public static class RockCatalog
{
    private const string EmbeddedResourceName = "RockRecommender.Infrastructure.Catalog.rock-catalog.json";

    private static readonly RockCatalogDocument Document = LoadDocument();

    public static IReadOnlyList<string> Genres => Document.Genres;

    public static IReadOnlyList<string> Bands { get; } = [.. Document.Bands.Select(band => band.Name)];

    public static List<Song> GetSongs() => BuildSongs();

    private static RockCatalogDocument LoadDocument()
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var stream = assembly.GetManifestResourceStream(EmbeddedResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedResourceName}' was not found.");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        return JsonSerializer.Deserialize<RockCatalogDocument>(stream, options)
            ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedResourceName}' could not be parsed.");
    }

    private static List<Song> BuildSongs()
    {
        var songs = new List<Song>();

        foreach (var band in Document.Bands)
        {
            foreach (var title in band.Songs)
            {
                songs.Add(Song.Create(BuildSongId(band.Name, title), title, band.Name, band.Genre).Value!);
            }
        }

        return songs;
    }

    private static Guid BuildSongId(string band, string title)
    {
        var input = Encoding.UTF8.GetBytes($"{band.ToLowerInvariant()}|{title.ToLowerInvariant()}");
        var hash = MD5.HashData(input);

        return new Guid(hash);
    }
}
