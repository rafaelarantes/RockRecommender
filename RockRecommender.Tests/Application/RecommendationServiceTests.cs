using RockRecommender.Application.Common;
using RockRecommender.Application.Recommendations;
using RockRecommender.Domain.Entities;
using RockRecommender.Tests.Fakes;

namespace RockRecommender.Tests.Application;

public class RecommendationServiceTests
{
    [Fact]
    public async Task GetNextSongAsync_WithNoFeedback_RecommendsSongFromLikedBand()
    {
        var songRepository = new FakeSongRepository();
        songRepository.Songs.Add(CreateSong(Guid.NewGuid(), "Master of Puppets", "Metallica", "Thrash Metal"));
        songRepository.Songs.Add(CreateSong(Guid.NewGuid(), "Paranoid", "Black Sabbath", "Heavy Metal"));
        songRepository.Songs.Add(CreateSong(Guid.NewGuid(), "Enter Sandman", "Metallica", "Thrash Metal"));
        var userRepository = new FakeUserRepository();
        var user = CreateUser(Guid.NewGuid(), ["Metallica"]);
        userRepository.UsersById[user.Id] = user;
        var service = new RecommendationService(
            songRepository,
            userRepository,
            new FakeFeedbackRepository(),
            new FakeRecommendationLogRepository(),
            new FakeCollaborativeRecommender());

        var result = await service.GetNextSongAsync(user.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Metallica", result.Value!.Band);
    }

    [Fact]
    public async Task GetNextSongAsync_DoesNotRecommendSongAlreadyInTheLog()
    {
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var song3Id = Guid.NewGuid();
        var songRepository = new FakeSongRepository();
        songRepository.Songs.Add(CreateSong(song1Id, "Song One", "Band A", "Classic Rock"));
        songRepository.Songs.Add(CreateSong(song2Id, "Song Two", "Band B", "Punk Rock"));
        songRepository.Songs.Add(CreateSong(song3Id, "Song Three", "Band C", "Grunge"));
        var userRepository = new FakeUserRepository();
        var user = CreateUser(Guid.NewGuid(), []);
        userRepository.UsersById[user.Id] = user;
        var recommendationLogRepository = new FakeRecommendationLogRepository();
        recommendationLogRepository.Entries.Add(new RecommendationLogEntry(user.Id, song1Id, DateTime.UtcNow));
        recommendationLogRepository.Entries.Add(new RecommendationLogEntry(user.Id, song2Id, DateTime.UtcNow));
        var service = new RecommendationService(
            songRepository,
            userRepository,
            new FakeFeedbackRepository(),
            recommendationLogRepository,
            new FakeCollaborativeRecommender());

        var result = await service.GetNextSongAsync(user.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(song3Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetNextSongAsync_WithFeedback_UsesCollaborativeModelToPickHighestScoringCandidate()
    {
        var song1Id = Guid.NewGuid();
        var song2Id = Guid.NewGuid();
        var song3Id = Guid.NewGuid();
        var songRepository = new FakeSongRepository();
        songRepository.Songs.Add(CreateSong(song1Id, "Song One", "Band A", "Classic Rock"));
        songRepository.Songs.Add(CreateSong(song2Id, "Song Two", "Band B", "Punk Rock"));
        songRepository.Songs.Add(CreateSong(song3Id, "Song Three", "Band C", "Grunge"));
        var userRepository = new FakeUserRepository();
        var user = CreateUser(Guid.NewGuid(), []);
        userRepository.UsersById[user.Id] = user;
        var feedbackRepository = new FakeFeedbackRepository();
        feedbackRepository.Items.Add(new Feedback(user.Id, song1Id, true, DateTime.UtcNow));
        var collaborativeRecommender = new FakeCollaborativeRecommender
        {
            IsAvailable = true,
            ScoreFunc = (_, songId) => songId == song2Id ? 0.9f : 0.1f,
        };
        var service = new RecommendationService(
            songRepository,
            userRepository,
            feedbackRepository,
            new FakeRecommendationLogRepository(),
            collaborativeRecommender);

        var result = await service.GetNextSongAsync(user.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal(song2Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetNextSongAsync_WithFeedbackAndUnavailableModel_ReturnsUnavailableResult()
    {
        var song1Id = Guid.NewGuid();
        var songRepository = new FakeSongRepository();
        songRepository.Songs.Add(CreateSong(song1Id, "Song One", "Band A", "Classic Rock"));
        var userRepository = new FakeUserRepository();
        var user = CreateUser(Guid.NewGuid(), []);
        userRepository.UsersById[user.Id] = user;
        var feedbackRepository = new FakeFeedbackRepository();
        feedbackRepository.Items.Add(new Feedback(user.Id, song1Id, true, DateTime.UtcNow));
        var service = new RecommendationService(
            songRepository,
            userRepository,
            feedbackRepository,
            new FakeRecommendationLogRepository(),
            new FakeCollaborativeRecommender { IsAvailable = false });

        var result = await service.GetNextSongAsync(user.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultFailure.Unavailable, result.Failure);
    }

    private static Song CreateSong(Guid id, string title, string band, string genre) =>
        Song.Create(id, title, band, genre).Value!;

    private static User CreateUser(Guid id, IEnumerable<string> likedBandIds) =>
        User.Create(id, likedBandIds).Value!;
}
