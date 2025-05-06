using Contracts.DTO.Auth;
using Model.Entities;

namespace API.Helpers.Mappers
{
    public static class AccountMapper
    {
        public static Account ToModel(this RegisterRequest dto)
        {
            return new Account
            {
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role.ToModel()
            };
        }

        public static Person ToPerson(this RegisterRequest dto, int accountId)
        {
            Person person = dto.Role switch
            {
                Contracts.Enums.UserRole.Driver => new Driver(),
                Contracts.Enums.UserRole.Coach => new Coach(),
                _ => new Person()
            };

            person.FirstName = dto.FirstName;
            person.Prefix = dto.Prefix ?? string.Empty;
            person.LastName = dto.LastName;
            person.DateOfBirth = dto.DateOfBirth;
            person.AccountID = accountId;

            return person;
        }

        public static AuthResponse ToAuthResponse(this Account account, string token)
        {
            return new AuthResponse
            {
                Token = token
            };
        }
    }
}