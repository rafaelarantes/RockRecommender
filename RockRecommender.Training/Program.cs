using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RockRecommender.Training;
using RockRecommender.Training.Synthetic;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<TrainingOptions>(builder.Configuration.GetSection(TrainingOptions.SectionName));
builder.Services.Configure<SyntheticInteractionOptions>(builder.Configuration.GetSection(SyntheticInteractionOptions.SectionName));
builder.Services.AddSingleton<SyntheticInteractionGenerator>();
builder.Services.AddSingleton<TrainingPipeline>();

using var app = builder.Build();

app.Services.GetRequiredService<TrainingPipeline>().Run();
