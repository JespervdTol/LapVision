using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public enum UserRole
    {
        Driver,
        Coach,
        Admin
    }

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

        // One-to-one navigation
        public Person Person { get; set; }
    }
}