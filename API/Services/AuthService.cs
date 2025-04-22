using Infrastructure.Persistence;
using Model.Entities;
using Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contracts.DTO.Auth;
using API.Helpers.Mappers;
using API.Helpers;

namespace API.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public AuthService(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<Account?> RegisterAsync(string email, string username, string password, UserRole role, RegisterRequest request)
        {
            if (await _context.Accounts.AnyAsync(a => a.Email == email))
                return null;

            var hashedPassword = HashPassword(password);

            var account = new Account
            {
                Email = email,
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var person = request.ToPerson(account.AccountID);
            person.ProfilePicture = ImageDefaults.DefaultProfilePicture;
            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Account?> LoginAsync(string emailOrUsername, string password)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == emailOrUsername)
                ?? await _context.Accounts.FirstOrDefaultAsync(a => a.Username == emailOrUsername);

            if (account == null || !VerifyPassword(password, account.PasswordHash))
                return null;

            return account;
        }

        public string GenerateJwtToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddHours(3);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountID.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}