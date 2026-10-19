using norway_weather_api.DTOs;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace norway_weather_api.Services;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<GeocodingResultDto>> SearchCitiesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var url =
            $"search?format=jsonv2&limit=5&q={Uri.EscapeDataString(query)}";

        using var response = await _httpClient.GetAsync(
            url,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var results = await response.Content
            .ReadFromJsonAsync<List<NominatimResult>>(
                cancellationToken: cancellationToken);

        return (results ?? [])
            .Select(result => new GeocodingResultDto
            {
                DisplayName = result.DisplayName,
                Latitude = double.Parse(
                    result.Latitude,
                    CultureInfo.InvariantCulture),
                Longitude = double.Parse(
                    result.Longitude,
                    CultureInfo.InvariantCulture)
            })
            .ToList();
    }

    private class NominatimResult
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public string Latitude { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } = string.Empty;
    }
}