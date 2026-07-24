namespace RockRecommender.Training.Evaluation;

public sealed record LeaveOneOutResult(double PrecisionAtK, double RecallAtK, double NdcgAtK, int EvaluatedUserCount);
