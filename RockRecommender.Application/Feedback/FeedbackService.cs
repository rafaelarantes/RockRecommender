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
}
