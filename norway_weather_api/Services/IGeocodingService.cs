using norway_weather_api.DTOs;

namespace norway_weather_api.Services
{
    public interface IGeocodingService
    {
        Task<IReadOnlyList<GeocodingResultDto>> SearchCitiesAsync(
            string query,
            CancellationToken cancellationToken = default);
    }
}
