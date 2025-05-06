using CoachWeb.Infrastructure.Interfaces;
using CoachWeb.Models.Account;
using Model.Entities;
using Model.Enums;
using MySqlConnector;

namespace CoachWeb.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AccountRepository(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<Account?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT AccountID, Username, Email, PasswordHash, Role 
                FROM Account 
                WHERE Email = @Input OR Username = @Input", conn);

            cmd.Parameters.AddWithValue("@Input", emailOrUsername);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Account
                {
                    AccountID = reader.GetInt32("AccountID"),
                    Username = reader.GetString("Username"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    Role = (UserRole)reader.GetInt32("Role")
                };
            }

            return null;
        }

        public async Task<Account?> CreateCoachAccountAsync(RegisterCoachViewModel vm, string hash)
        {
            var connString = _config.GetConnectionString("DefaultConnection");
            using var conn = new MySqlConnection(connString);
            await conn.OpenAsync();
            using var tran = await conn.BeginTransactionAsync();

            try
            {
                var cmd1 = new MySqlCommand(@"
                    INSERT INTO Account (Username, Email, PasswordHash, Role, CreatedAt)
                    VALUES (@Username, @Email, @PasswordHash, @Role, @Now);
                    SELECT LAST_INSERT_ID();", conn, (MySqlTransaction)tran);

                cmd1.Parameters.AddWithValue("@Username", vm.Username);
                cmd1.Parameters.AddWithValue("@Email", vm.Email);
                cmd1.Parameters.AddWithValue("@PasswordHash", hash);
                cmd1.Parameters.AddWithValue("@Role", (int)Model.Enums.UserRole.Coach);
                cmd1.Parameters.AddWithValue("@Now", DateTime.UtcNow);

                var accountId = Convert.ToInt32(await cmd1.ExecuteScalarAsync());

                var cmd2 = new MySqlCommand(@"
                    INSERT INTO Person (FirstName, Prefix, LastName, DateOfBirth, AccountID, PersonType)
                    VALUES (@FirstName, @Prefix, @LastName, @DOB, @AccountID, @PersonType);", conn, (MySqlTransaction)tran);

                cmd2.Parameters.AddWithValue("@FirstName", vm.FirstName);
                cmd2.Parameters.AddWithValue("@Prefix", vm.Prefix);
                cmd2.Parameters.AddWithValue("@LastName", vm.LastName);
                cmd2.Parameters.AddWithValue("@DOB", vm.DateOfBirth);
                cmd2.Parameters.AddWithValue("@AccountID", accountId);
                cmd2.Parameters.AddWithValue("@PersonType", "Coach");

                await cmd2.ExecuteNonQueryAsync();
                await tran.CommitAsync();

                return new Account
                {
                    AccountID = accountId,
                    Username = vm.Username,
                    Email = vm.Email,
                    PasswordHash = hash,
                    Role = Model.Enums.UserRole.Coach,
                    CreatedAt = DateTime.UtcNow
                };
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