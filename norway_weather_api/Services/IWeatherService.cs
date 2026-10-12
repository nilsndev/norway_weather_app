
using norway_weather_api.DTOs;

namespace norway_weather_api.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponseDto?> GetCurrentWeatherAsync(
            double latitude, double longitude);

        Task<List<WeatherResponseDto>> GetForecastAsync(
            double latitude, double longitude);
    }
}
