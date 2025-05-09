using System.ComponentModel.DataAnnotations;

namespace Contracts.CoachWeb.ViewModels.Account
{
    public class RegisterCoachViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string Prefix { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
