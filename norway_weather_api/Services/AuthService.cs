using Microsoft.AspNetCore.Identity;
using norway_weather_api.Models;
using norway_weather_api.DTOs;
using System.Security.Claims;

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

        public async Task<ApplicationUser?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }
            var validPW = await _userManager.CheckPasswordAsync(user, request.Password);
               
            if(!validPW){
                return null;
            }

            return user;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }
    }
}
