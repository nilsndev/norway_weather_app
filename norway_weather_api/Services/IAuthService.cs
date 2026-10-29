using norway_weather_api.DTOs;
using norway_weather_api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace norway_weather_api.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequestDto request);

        Task<ApplicationUser?> LoginAsync(LoginRequestDto request);

        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal);
    }
}
