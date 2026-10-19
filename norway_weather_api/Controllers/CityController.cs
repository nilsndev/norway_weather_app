using Microsoft.AspNetCore.Mvc;
using norway_weather_api.DTOs;
using norway_weather_api.Services;
using System.Text.Json.Serialization;

namespace norway_weather_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CityController : ControllerBase
    {
        private readonly IGeocodingService _geocodingService;

        public CityController(IGeocodingService geocodingService)
        {
            _geocodingService = geocodingService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IReadOnlyList<GeocodingResultDto>>>
        SearchCities(
            [FromQuery] string? query,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length > 100)
            {
                return BadRequest(
                    "Query must contain between 1 and 100 characters.");
            }

            var results = await _geocodingService.SearchCitiesAsync(
                query.Trim(),
                cancellationToken);

            return Ok(results);
        }


    }
}
