using Model.Enums;

namespace Model.Entities.CoachWeb
{
    public class Account
    {
        public int AccountID { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Person Person { get; private set; }

        public Account(string username, string email, string passwordHash, UserRole role)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetPerson(Person person)
        {
            Person = person;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

        public void UpdatePassword(string hash)
        {
            PasswordHash = hash;
        }
    }
}