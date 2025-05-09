using Infrastructure.CoachWeb.Interfaces;
using Microsoft.Extensions.Configuration;
using Model.Entities.CoachWeb;
using Model.Enums;
using MySqlConnector;

namespace Infrastructure.CoachWeb.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<Account?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT AccountID, Username, Email, PasswordHash, Role, CreatedAt
                FROM Account 
                WHERE Email = @Input OR Username = @Input", conn);

            cmd.Parameters.AddWithValue("@Input", emailOrUsername);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var account = new Account(
                    reader.GetString("Username"),
                    reader.GetString("Email"),
                    reader.GetString("PasswordHash"),
                    (UserRole)reader.GetInt32("Role")
                );

                typeof(Account).GetProperty("AccountID")?.SetValue(account, reader.GetInt32("AccountID"));
                typeof(Account).GetProperty("CreatedAt")?.SetValue(account, reader.GetDateTime("CreatedAt"));

                return account;
            }

            return null;
        }

        public async Task<Account?> CreateCoachAccountAsync(Account account, Person person)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();
            using var tran = await conn.BeginTransactionAsync();

            try
            {
                var cmd1 = new MySqlCommand(@"
                    INSERT INTO Account (Username, Email, PasswordHash, Role, CreatedAt)
                    VALUES (@Username, @Email, @PasswordHash, @Role, @Now);
                    SELECT LAST_INSERT_ID();", conn, tran);

                cmd1.Parameters.AddWithValue("@Username", account.Username);
                cmd1.Parameters.AddWithValue("@Email", account.Email);
                cmd1.Parameters.AddWithValue("@PasswordHash", account.PasswordHash);
                cmd1.Parameters.AddWithValue("@Role", (int)account.Role);
                cmd1.Parameters.AddWithValue("@Now", account.CreatedAt);

                var accountId = Convert.ToInt32(await cmd1.ExecuteScalarAsync());
                typeof(Account).GetProperty("AccountID")?.SetValue(account, accountId);

                var cmd2 = new MySqlCommand(@"
                    INSERT INTO Person (FirstName, Prefix, LastName, DateOfBirth, AccountID, PersonType)
                    VALUES (@FirstName, @Prefix, @LastName, @DOB, @AccountID, @PersonType);", conn, tran);

                cmd2.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd2.Parameters.AddWithValue("@Prefix", person.Prefix);
                cmd2.Parameters.AddWithValue("@LastName", person.LastName);
                cmd2.Parameters.AddWithValue("@DOB", person.DateOfBirth);
                cmd2.Parameters.AddWithValue("@AccountID", accountId);
                cmd2.Parameters.AddWithValue("@PersonType", "Coach");

                await cmd2.ExecuteNonQueryAsync();
                await tran.CommitAsync();

                return account;
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"❌ Registration error: {ex.Message}");
                return null;
            }
        }
    }
}