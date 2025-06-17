using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.Interfaces.Repositories;
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

        public async Task<Result<Account>> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            try
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

                    return Result<Account>.Success(account);
                }

                return Result<Account>.Failure("User not found.", ErrorType.UserError);
            }
            catch (Exception ex)
            {
                return Result<Account>.Failure("Database error while retrieving user.", ErrorType.SystemError);
            }
        }

        public async Task<Result<Account>> CreateCoachAccountAsync(Account account, Person person)
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                await conn.OpenAsync();
                using var tran = await conn.BeginTransactionAsync();

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

                return Result<Account>.Success(account);
            }
            catch
            {
                return Result<Account>.Failure("An error occurred during registration. Please try again later.");
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("SELECT 1 FROM Account WHERE Email = @Email LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@Email", email);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new MySqlCommand("SELECT 1 FROM Account WHERE Username = @Username LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@Username", username);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
    }
}