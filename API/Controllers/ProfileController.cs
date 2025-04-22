using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Contracts.DTO.Auth;

namespace API.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var accountIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized();

            var person = await _context.People
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountID == accountId);

            if (person == null)
                return NotFound("Profile not found.");

            return Ok(new UserProfileDTO
            {
                FirstName = person.FirstName,
                Prefix = person.Prefix,
                LastName = person.LastName,
                DateOfBirth = person.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Email = person.Account.Email,
                Username = person.Account.Username,
                Role = person.Account.Role.ToString(),
                ProfilePicture = person.ProfilePicture != null
                    ? Convert.ToBase64String(person.ProfilePicture)
                    : null
            });
        }

        [HttpPut("picture")]
        public async Task<IActionResult> UploadPicture([FromBody] byte[] imageData)
        {
            var accountIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized();

            var person = await _context.People.FirstOrDefaultAsync(p => p.AccountID == accountId);
            if (person == null)
                return NotFound();

            person.ProfilePicture = imageData;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}