namespace norway_weather_api.Services
{
    public interface IWeatherService
    {
        Task<string> GetWeatherAsync(double latitude, double longitude);
    }
}
