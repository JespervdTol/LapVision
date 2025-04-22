namespace Contracts.DTO.Auth
{
    public class UserProfileDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string? Prefix { get; set; }
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string? ProfilePicture { get; set; }
    }
}