namespace RockRecommender.Domain.Evaluation;

public static class RankingMetrics
{
    public static double PrecisionAtK(IReadOnlyList<Guid> rankedItemIds, IReadOnlySet<Guid> relevantItemIds, int k)
    {
        if (k <= 0)
            return 0d;

        var topK = rankedItemIds.Take(k).ToList();
        
        if (topK.Count == 0)
            return 0d;

        var hits = topK.Count(itemId => relevantItemIds.Contains(itemId));
        
        return (double)hits / topK.Count;
    }

    public static double RecallAtK(IReadOnlyList<Guid> rankedItemIds, IReadOnlySet<Guid> relevantItemIds, int k)
    {
        if (k <= 0 || relevantItemIds.Count == 0)
            return 0d;

        var topK = rankedItemIds.Take(k);
        var hits = topK.Count(itemId => relevantItemIds.Contains(itemId));
        
        return (double)hits / relevantItemIds.Count;
    }

    public static double NdcgAtK(IReadOnlyList<Guid> rankedItemIds, IReadOnlySet<Guid> relevantItemIds, int k)
    {
        if (k <= 0 || relevantItemIds.Count == 0)
            return 0d;

        var topK = rankedItemIds.Take(k).ToList();

        var discountedGain = 0d;
        
        for (var position = 0; position < topK.Count; position++)
        {
            if (relevantItemIds.Contains(topK[position]))
                discountedGain += 1d / Math.Log2(position + 2);
        }

        var idealHits = Math.Min(relevantItemIds.Count, k);
        var idealDiscountedGain = 0d;

        for (var position = 0; position < idealHits; position++)
            idealDiscountedGain += 1d / Math.Log2(position + 2);

        return idealDiscountedGain == 0d ? 0d : discountedGain / idealDiscountedGain;
    }
}
