using Infrastructure.Persistence;
using Model.Entities;
using Model.Enums;

namespace API.Helpers
{
    public static class DataSeeder
    {
        public static void SeedAdmin(DataContext context, IConfiguration config)
        {
            if (context.Accounts.Any()) return;

            var adminSection = config.GetSection("SeedAdmin");

            var email = adminSection["Email"];
            var username = adminSection["Username"];
            var password = adminSection["Password"];
            var firstName = adminSection["FirstName"];
            var lastName = adminSection["LastName"];
            var dob = DateOnly.Parse(adminSection["DateOfBirth"]);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var admin = new Account
            {
                Email = email,
                Username = username,
                PasswordHash = passwordHash,
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                Person = new Person
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Prefix = "",
                    DateOfBirth = dob
                }
            };

            context.Accounts.Add(admin);
            context.SaveChanges();

            System.Diagnostics.Debug.WriteLine($"✅ Admin seeded: {email}");
        }
    }
}