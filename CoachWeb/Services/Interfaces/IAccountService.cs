using Contracts.CoachWeb.ViewModels.Account;
using Model.Entities.CoachWeb;

namespace CoachWeb.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> ValidateLoginAsync(string emailOrUsername, string password);
        Task<Account?> RegisterCoachAsync(RegisterCoachViewModel vm);
    }
}