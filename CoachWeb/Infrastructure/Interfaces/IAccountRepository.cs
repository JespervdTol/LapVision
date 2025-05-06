using Model.Entities;
using CoachWeb.Models.Account;

namespace CoachWeb.Infrastructure.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByEmailOrUsernameAsync(string emailOrUsername);

        Task<Account?> CreateCoachAccountAsync(RegisterCoachViewModel vm, string hashedPassword);
    }
}