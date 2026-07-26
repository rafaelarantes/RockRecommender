using RockRecommender.Application.Catalog;
using RockRecommender.Tests.Fakes;

namespace RockRecommender.Tests.Application;

public class BandServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsEveryBandFromTheRepository()
    {
        var bandRepository = new FakeBandRepository();
        bandRepository.Bands.Add("Metallica");
        bandRepository.Bands.Add("Black Sabbath");
        var service = new BandService(bandRepository);

        var bands = await service.GetAllAsync();

        Assert.Equal(["Metallica", "Black Sabbath"], bands);
    }
}
