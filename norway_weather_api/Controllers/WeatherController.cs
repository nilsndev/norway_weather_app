using Microsoft.AspNetCore.Mvc;
using norway_weather_api.DTOs;
using norway_weather_api.Services;

namespace norway_weather_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] WeatherRequestDto request)
        {
            var weather = await _weatherService.GetWeatherAsync(
            request.Latitude!.Value,
            request.Longitude!.Value);

            return Content(weather, "application/json");
        }
    }
}
