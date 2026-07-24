using Microsoft.AspNetCore.Mvc;
using RockRecommender.Application.Common;

namespace RockRecommender.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Failure switch
        {
            ResultFailure.Invalid => BadRequest(result.Error),
            ResultFailure.NotFound => NotFound(result.Error),
            ResultFailure.Conflict => Conflict(result.Error),
            ResultFailure.Unavailable => StatusCode(StatusCodes.Status503ServiceUnavailable, result.Error),
            _ => Problem(result.Error),
        };
    }
}
