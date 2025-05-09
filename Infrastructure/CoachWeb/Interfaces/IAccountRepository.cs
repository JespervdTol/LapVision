using Model.Entities.CoachWeb;

namespace Infrastructure.CoachWeb.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByEmailOrUsernameAsync(string emailOrUsername);
        Task<Account?> CreateCoachAccountAsync(Account account, Person person);
    }
}