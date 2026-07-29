using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockRecommender.Application.Feedback;
using RockRecommender.Domain.Repositories;
using RockRecommender.Infrastructure.Mongo;
using RockRecommender.Infrastructure.Mongo.Repositories;
using RockRecommender.Training;
using RockRecommender.Training.RealFeedback;
using RockRecommender.Training.Synthetic;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<TrainingOptions>(builder.Configuration.GetSection(TrainingOptions.SectionName));
builder.Services.Configure<SyntheticInteractionOptions>(builder.Configuration.GetSection(SyntheticInteractionOptions.SectionName));
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection(MongoOptions.SectionName));

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddSingleton<SyntheticInteractionGenerator>();
builder.Services.AddScoped<InteractionSourceSelector>();
builder.Services.AddScoped<TrainingPipeline>();
builder.Services.AddHostedService<RetrainingBackgroundService>();

using var app = builder.Build();

await app.RunAsync();
