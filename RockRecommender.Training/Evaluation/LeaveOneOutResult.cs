namespace RockRecommender.Training.Evaluation;

public sealed record LeaveOneOutResult(double PrecisionAtK, double RecallAtK, double NdcgAtK, int EvaluatedUserCount)
{
    public bool IsBetterThan(LeaveOneOutResult other) =>
        PrecisionAtK >= other.PrecisionAtK &&
        RecallAtK >= other.RecallAtK &&
        NdcgAtK >= other.NdcgAtK &&
        (PrecisionAtK > other.PrecisionAtK || RecallAtK > other.RecallAtK || NdcgAtK > other.NdcgAtK);
}
