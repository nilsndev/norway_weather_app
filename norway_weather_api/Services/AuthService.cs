using Microsoft.AspNetCore.Identity;
using norway_weather_api.Models;
using norway_weather_api.DTOs;

namespace norway_weather_api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        public async Task <bool> RegisterAsync(RegisterRequestDto request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            return result.Succeeded;
        }
    }
}
