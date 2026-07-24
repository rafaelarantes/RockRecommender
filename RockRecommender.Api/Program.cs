using RockRecommender.Api.Extensions;
using RockRecommender.Application.Feedback;
using RockRecommender.Application.Recommendations;
using RockRecommender.Application.Users;
using RockRecommender.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRockRecommenderInfrastructure(builder.Configuration);
builder.Services.AddScoped<RecommendationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<FeedbackService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

await app.SeedCatalogAsync();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RockRecommender API v1");
});
app.MapControllers();

app.Run();
