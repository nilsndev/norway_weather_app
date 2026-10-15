using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using norway_weather_api.DTOs;
using norway_weather_api.Services;
using System.Security.Claims;

namespace norway_weather_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public AuthController(IAuthService authService, ITokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result)
            {
                return BadRequest("Registration failed.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request){
             var user = await _authService.LoginAsync(request);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                token
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _authService.GetCurrentUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                user.Id,
                user.Email
            });
        }
    }
}
