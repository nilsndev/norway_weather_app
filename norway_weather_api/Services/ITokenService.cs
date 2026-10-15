using norway_weather_api.Models;

namespace norway_weather_api.Services
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
