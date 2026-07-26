using Microsoft.AspNetCore.Mvc;
using RockRecommender.Application.Catalog;

namespace RockRecommender.Api.Controllers;

[Route("bands")]
public sealed class BandsController(BandService bandService) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetBands()
    {
        var bands = await bandService.GetAllAsync();

        return Ok(bands);
    }
}
