
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using norway_weather_api.DTOs;

namespace norway_weather_api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(25);

        public WeatherService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<WeatherResponseDto?> GetCurrentWeatherAsync(
            double latitude, double longitude)
        {
            var forecasts = await GetAllForecastsAsync(latitude, longitude);

            return forecasts.FirstOrDefault();
        }

        public async Task<List<WeatherResponseDto>> GetForecastAsync(
            double latitude, double longitude)
        {
            var forecasts = await GetAllForecastsAsync(latitude, longitude);

            if (forecasts.Count == 0)
                return forecasts;

            var start = new DateTimeOffset(forecasts[0].Time);
            var end = start.AddHours(24);

            return forecasts
                .Where(f =>
                {
                    var time = new DateTimeOffset(f.Time);
                    return time >= start && time <= end;
                })
                .ToList();
        }

        private async Task<List<WeatherResponseDto>> GetAllForecastsAsync(
            double latitude, double longitude)
        {
            var lat = Math.Round(latitude, 4);
            var lon = Math.Round(longitude, 4);

            var cacheKey = string.Create(
                CultureInfo.InvariantCulture,
                $"weather:{lat:F4}:{lon:F4}");

            var cached = _cache.Get<List<WeatherResponseDto>>(cacheKey);

            if (cached is not null)
                return cached;

            var latText = lat.ToString(CultureInfo.InvariantCulture);
            var lonText = lon.ToString(CultureInfo.InvariantCulture);

            var url =
                $"weatherapi/locationforecast/2.0/compact?lat={latText}&lon={lonText}";

            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);

            var timeseries = json.RootElement
                .GetProperty("properties")
                .GetProperty("timeseries");

            var forecasts = timeseries.EnumerateArray()
                .Select(MapForecast)
                .ToList();

            _cache.Set(
                cacheKey,
                forecasts,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });

            return forecasts;
        }

        private static WeatherResponseDto MapForecast(JsonElement forecast)
        {
            var data = forecast.GetProperty("data");
            var details = data.GetProperty("instant")
                .GetProperty("details");

            JsonElement? period = null;

            foreach (var name in new[]
            {
                "next_1_hours", "next_6_hours", "next_12_hours"
            })
            {
                if (data.TryGetProperty(name, out var value))
                {
                    period = value;
                    break;
                }
            }

            string? symbol = null;
            double? precipitation = null;

            if (period is { } p)
            {
                if (p.TryGetProperty("summary", out var summary) &&
                    summary.TryGetProperty("symbol_code", out var symbolValue))
                {
                    symbol = symbolValue.GetString();
                }

                if (p.TryGetProperty("details", out var periodDetails) &&
                    periodDetails.TryGetProperty(
                        "precipitation_amount", out var amount) &&
                    amount.ValueKind == JsonValueKind.Number)
                {
                    precipitation = amount.GetDouble();
                }
            }

            return new WeatherResponseDto
            {
                Time = forecast.GetProperty("time").GetDateTime(),
                Temperature = details.GetProperty("air_temperature").GetDouble(),
                WindSpeed = details.GetProperty("wind_speed").GetDouble(),
                WindDirection = details.GetProperty("wind_from_direction").GetDouble(),
                Humidity = details.GetProperty("relative_humidity").GetDouble(),
                CloudCover = details.GetProperty("cloud_area_fraction").GetDouble(),
                WeatherSymbol = symbol,
                PrecipitationAmount = precipitation
            };
        }
    }
}
