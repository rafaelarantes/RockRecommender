using Microsoft.AspNetCore.Mvc;
using RockRecommender.Application.Dtos;
using RockRecommender.Application.Feedback;
using RockRecommender.Application.Recommendations;
using RockRecommender.Application.Users;

namespace RockRecommender.Api.Controllers;

[Route("users")]
public sealed class UsersController(
    UserService userService,
    FeedbackService feedbackService,
    RecommendationService recommendationService) : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request)
    {
        var result = await userService.CreateUserAsync(request);

        if (!result.IsSuccess)
            return ToActionResult(result);

        return Created($"/users/{result.Value!.Id}", result.Value);
    }

    [HttpGet("{userId:guid}/next-song")]
    public async Task<ActionResult<SongResponse>> GetNextSong(Guid userId)
    {
        var result = await recommendationService.GetNextSongAsync(userId);

        return ToActionResult(result);
    }

    [HttpPost("{userId:guid}/feedback")]
    public async Task<IActionResult> SubmitFeedback(Guid userId, FeedbackRequest request)
    {
        await feedbackService.SubmitFeedbackAsync(userId, request);

        return NoContent();
    }
}
