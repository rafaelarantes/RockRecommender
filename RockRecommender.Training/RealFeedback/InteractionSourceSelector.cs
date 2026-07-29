using RockRecommender.Application.Feedback;
using RockRecommender.Domain.Entities;
using RockRecommender.Training.Synthetic;

namespace RockRecommender.Training.RealFeedback;

public sealed class InteractionSourceSelector(FeedbackService feedbackService, SyntheticInteractionGenerator syntheticGenerator)
{
    public async Task<TrainingInteractions> SelectAsync(List<Song> songs, int syntheticUserCount)
    {
        var syntheticInteractions = syntheticGenerator.Generate(songs, syntheticUserCount);
        var realInteractions = await LoadRealInteractionsAsync();

        return realInteractions.Count > syntheticInteractions.Count
            ? new TrainingInteractions(realInteractions, IsReal: true)
            : new TrainingInteractions(syntheticInteractions, IsReal: false);
    }

    private async Task<List<Interaction>> LoadRealInteractionsAsync()
    {
        var feedback = await feedbackService.GetAllAsync();

        return RealInteractionMapper.ToInteractions(feedback);
    }
}
