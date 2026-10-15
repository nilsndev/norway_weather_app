
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentWeather(
            [FromQuery] WeatherRequestDto request)
        {
            var weather = await _weatherService.GetCurrentWeatherAsync(
                request.Latitude!.Value,
                request.Longitude!.Value);

            if (weather == null)
                return NotFound("No weather data available.");

            return Ok(weather);
        }

        [Authorize]
        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast(
            [FromQuery] WeatherRequestDto request)
        {
            var forecast = await _weatherService.GetForecastAsync(
                request.Latitude!.Value,
                request.Longitude!.Value);

            return Ok(forecast);
        }
    }
}
