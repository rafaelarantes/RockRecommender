using RockRecommender.Domain.Entities;
using RockRecommender.Domain.Repositories;

namespace RockRecommender.Tests.Fakes;

public sealed class FakeSongRepository : ISongRepository
{
    public List<Song> Songs { get; } = [];

    public Task<List<Song>> GetAllAsync() => Task.FromResult(Songs.ToList());
}
