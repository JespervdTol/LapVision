using Contracts.CoachWeb.ErrorHandeling;
using Model.Entities.CoachWeb;

namespace Infrastructure.CoachWeb.Interfaces
{
    public interface IAccountRepository
    {
        Task<Result<Account>> GetByEmailOrUsernameAsync(string emailOrUsername);
        Task<Result<Account>> CreateCoachAccountAsync(Account account, Person person);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
}