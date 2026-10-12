using System.Net.Http;
using System.Globalization;
using norway_weather_api.Services;

namespace norway_weather_api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetWeatherAsync(double latitude, double longitude)
        {
       
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"weatherapi/locationforecast/2.0/compact?lat={lat}&lon={lon}";

            using var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
