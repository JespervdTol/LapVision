using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.ViewModels.Account;
using Model.Entities.CoachWeb;

namespace Contracts.CoachWeb.Interfaces.Services
{
    public interface IAccountService
    {
        Task<Result<Account>> ValidateLoginAsync(string emailOrUsername, string password);
        Task<Result<Account>> RegisterCoachAsync(RegisterCoachViewModel vm);
    }
}