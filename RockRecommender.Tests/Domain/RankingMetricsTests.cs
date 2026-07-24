using RockRecommender.Domain.Evaluation;

namespace RockRecommender.Tests.Domain;

public class RankingMetricsTests
{
    [Fact]
    public void PrecisionAtK_CountsRelevantItemsAmongTopK()
    {
        //Arrange
        var s1 = Guid.NewGuid();
        var s2 = Guid.NewGuid();
        var s3 = Guid.NewGuid();
        var s4 = Guid.NewGuid();
        var s5 = Guid.NewGuid();
        List<Guid> ranked = [s1, s2, s3, s4, s5];
        HashSet<Guid> relevant = [s2, s4];

        //Act
        var precision = RankingMetrics.PrecisionAtK(ranked, relevant, 5);

        //Assert
        Assert.Equal(0.4, precision, precision: 4);
    }

    [Fact]
    public void PrecisionAtK_WithNoRankedItems_ReturnsZero()
    {
        //Arrange
        List<Guid> ranked = [];
        HashSet<Guid> relevant = [Guid.NewGuid()];

        //Act
        var precision = RankingMetrics.PrecisionAtK(ranked, relevant, 5);

        //Assert
        Assert.Equal(0d, precision);
    }

    [Fact]
    public void RecallAtK_DividesHitsByTotalRelevantItems()
    {
        //Arrange
        var s1 = Guid.NewGuid();
        var s2 = Guid.NewGuid();
        var s3 = Guid.NewGuid();
        var s4 = Guid.NewGuid();
        var s5 = Guid.NewGuid();
        List<Guid> ranked = [s1, s2, s3, s4, s5];
        HashSet<Guid> relevant = [s2, s4];

        //Act
        var recall = RankingMetrics.RecallAtK(ranked, relevant, 5);

        //Assert
        Assert.Equal(1.0, recall, precision: 4);
    }

    [Fact]
    public void RecallAtK_WithNoRelevantItems_ReturnsZero()
    {
        //Arrange
        List<Guid> ranked = [Guid.NewGuid(), Guid.NewGuid()];
        HashSet<Guid> relevant = [];

        //Act
        var recall = RankingMetrics.RecallAtK(ranked, relevant, 5);

        //Assert
        Assert.Equal(0d, recall);
    }

    [Fact]
    public void NdcgAtK_RanksRelevantItemAtTopHigherThanNothing()
    {
        //Arrange
        var s1 = Guid.NewGuid();
        var s2 = Guid.NewGuid();
        var s3 = Guid.NewGuid();
        List<Guid> ranked = [s1, s2, s3];
        HashSet<Guid> relevant = [s1];

        //Act
        var ndcg = RankingMetrics.NdcgAtK(ranked, relevant, 3);

        //Assert
        Assert.Equal(1.0, ndcg, precision: 4);
    }

    [Fact]
    public void NdcgAtK_PenalizesRelevantItemRankedLower()
    {
        //Arrange
        var s1 = Guid.NewGuid();
        var s2 = Guid.NewGuid();
        var s3 = Guid.NewGuid();
        List<Guid> ranked = [s1, s2, s3];
        HashSet<Guid> relevant = [s3];

        //Act
        var ndcg = RankingMetrics.NdcgAtK(ranked, relevant, 3);

        //Assert
        Assert.Equal(0.5, ndcg, precision: 4);
    }
}
