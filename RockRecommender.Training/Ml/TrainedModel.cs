using Microsoft.ML;
using Microsoft.ML.Data;

namespace RockRecommender.Training.Ml;

public sealed record TrainedModel(ITransformer Model, DataViewSchema InputSchema);
