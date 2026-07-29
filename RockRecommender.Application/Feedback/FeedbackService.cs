using RockRecommender.Application.Dtos;
using RockRecommender.Domain.Repositories;
using DomainFeedback = RockRecommender.Domain.Entities.Feedback;

namespace RockRecommender.Application.Feedback;

public sealed class FeedbackService(IFeedbackRepository feedbackRepository)
{
    public async Task SubmitFeedbackAsync(Guid userId, FeedbackRequest request)
    {
        var feedback = new DomainFeedback(userId, request.SongId, request.Liked, DateTime.UtcNow);

        await feedbackRepository.AddAsync(feedback);
    }

    public async Task<List<FeedbackResponse>> GetAllAsync()
    {
        var feedback = await feedbackRepository.GetAllAsync();

        return [.. feedback.Select(ToResponse)];
    }

    private static FeedbackResponse ToResponse(DomainFeedback feedback) =>
        new(feedback.UserId, feedback.SongId, feedback.Liked);
}
