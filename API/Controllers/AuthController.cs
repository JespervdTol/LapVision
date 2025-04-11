﻿using Microsoft.AspNetCore.Mvc;
using Core.Services;
using Core.Model;
using API.DTO.Auth;

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
            var account = await _authService.RegisterAsync(request.Email, request.Username, request.Password, request.Role);

            if (account == null)
                return BadRequest("User with this email already exists.");

            var token = _authService.GenerateJwtToken(account);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var account = await _authService.LoginAsync(request.Identifier, request.Password);

            if (account == null)
                return Unauthorized("Invalid credentials.");

            var token = _authService.GenerateJwtToken(account);
            return Ok(new { token });
        }
    }
}