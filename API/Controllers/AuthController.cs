using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Helpers.Mappers;
using Contracts.App.DTO.Auth;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var modelRole = request.Role.ToModel();

            var account = await _authService.RegisterAsync(
                request.Email,
                request.Username,
                request.Password,
                modelRole,
                request
            );

            if (account == null)
                return BadRequest("User with this email already exists.");

            var token = _authService.GenerateJwtToken(account);
            return Ok(account.ToAuthResponse(token));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var account = await _authService.LoginAsync(request.Identifier, request.Password);

            if (account == null)
                return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJwtToken(account);
            return Ok(account.ToAuthResponse(token));
        }
    }
}