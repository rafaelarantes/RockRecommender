using RockRecommender.Infrastructure.Mongo;

namespace RockRecommender.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task SeedCatalogAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        try
        {
            var seeder = scope.ServiceProvider.GetRequiredService<MongoCatalogSeeder>();
            await seeder.SeedSongsIfEmptyAsync();
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Could not seed the song catalog. Is MongoDB running?");
        }
    }
}
