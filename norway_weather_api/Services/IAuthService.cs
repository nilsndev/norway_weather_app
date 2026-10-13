using norway_weather_api.DTOs;

namespace norway_weather_api.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequestDto request);
    }
}
