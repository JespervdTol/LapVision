using Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace Contracts.App.DTO.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        // New fields for Person
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? Prefix { get; set; }

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}