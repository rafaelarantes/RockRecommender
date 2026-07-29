using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockRecommender.Application.Recommendations;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Catalog;
using RockRecommender.Infrastructure.MachineLearning;
using RockRecommender.Infrastructure.Mongo;
using RockRecommender.Infrastructure.Mongo.Repositories;

namespace RockRecommender.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRockRecommenderInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoOptions>(configuration.GetSection(MongoOptions.SectionName));
        services.Configure<CollaborativeModelOptions>(configuration.GetSection(CollaborativeModelOptions.SectionName));

        services.AddSingleton<MongoContext>();
        services.AddSingleton<MongoCatalogSeeder>();
        services.AddSingleton<CollaborativeRecommenderModel>();
        services.AddSingleton<ICollaborativeRecommender>(provider => provider.GetRequiredService<CollaborativeRecommenderModel>());
        services.AddHostedService<ModelReloadBackgroundService>();

        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IRecommendationLogRepository, RecommendationLogRepository>();
        services.AddScoped<IBandRepository, BandRepository>();

        return services;
    }
}
