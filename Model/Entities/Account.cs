using Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{

    public class Account
    {
        [Key]
        public int AccountID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Person Person { get; set; }
    }
}