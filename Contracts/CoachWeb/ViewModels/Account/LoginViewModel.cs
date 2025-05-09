using System.ComponentModel.DataAnnotations;

namespace Contracts.CoachWeb.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email or Username")]
        public string EmailOrUsername { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ErrorMessage { get; set; }
    }
}