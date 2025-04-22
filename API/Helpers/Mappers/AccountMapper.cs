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
            return new Person
            {
                FirstName = dto.FirstName,
                Prefix = dto.Prefix ?? string.Empty,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                AccountID = accountId
            };
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