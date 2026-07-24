using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RockRecommender.Domain.Entities;
using RockRecommender.Infrastructure.Catalog;

namespace RockRecommender.Training.Synthetic;

public sealed class SyntheticInteractionGenerator(IOptions<SyntheticInteractionOptions> options)
{
    public List<SyntheticInteraction> Generate(List<Song> songs, int userCount, int seed = 42)
    {
        var settings = options.Value;
        var random = new Random(seed);
        var interactions = new List<SyntheticInteraction>();

        for (var userIndex = 0; userIndex < userCount; userIndex++)
        {
            var userId = BuildSyntheticUserId(userIndex);
            var preferredGenres = SelectPreferredGenres(random, settings);

            interactions.AddRange(GenerateInteractionsForUser(userId, songs, preferredGenres, random, settings));
        }

        return interactions;
    }

    private static Guid BuildSyntheticUserId(int userIndex)
    {
        var input = Encoding.UTF8.GetBytes($"synthetic-user-{userIndex}");
        var hash = MD5.HashData(input);

        return new Guid(hash);
    }

    private static HashSet<string> SelectPreferredGenres(Random random, SyntheticInteractionOptions settings)
    {
        var clusterSize = random.Next(settings.MinPreferredGenreCount, settings.MaxPreferredGenreCount + 1);

        return RockCatalog.Genres
            .OrderBy(_ => random.Next())
            .Take(clusterSize)
            .ToHashSet();
    }

    private static IEnumerable<SyntheticInteraction> GenerateInteractionsForUser(
        Guid userId,
        List<Song> songs,
        HashSet<string> preferredGenres,
        Random random,
        SyntheticInteractionOptions settings)
    {
        foreach (var song in songs)
        {
            var interaction = TryCreateInteraction(userId, song, preferredGenres, random, settings);

            if (interaction is not null)
            {
                yield return interaction;
            }
        }
    }

    private static SyntheticInteraction? TryCreateInteraction(
        Guid userId,
        Song song,
        HashSet<string> preferredGenres,
        Random random,
        SyntheticInteractionOptions settings)
    {
        var isPreferredGenre = preferredGenres.Contains(song.Genre);
        var interactionProbability = isPreferredGenre
            ? settings.PreferredGenreInteractionProbability
            : settings.OtherGenreInteractionProbability;

        if (random.NextDouble() > interactionProbability)
        {
            return null;
        }

        var likeProbability = isPreferredGenre ? settings.PreferredGenreLikeProbability : settings.OtherGenreLikeProbability;
        var liked = random.NextDouble() < likeProbability;

        return new SyntheticInteraction(userId, song.Id, liked);
    }
}
