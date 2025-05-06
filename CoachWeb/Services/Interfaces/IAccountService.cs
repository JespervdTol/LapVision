using Model.Entities;
using CoachWeb.Models.Account;

namespace CoachWeb.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Account?> ValidateLoginAsync(string emailOrUsername, string password);

        Task<Account?> RegisterCoachAsync(RegisterCoachViewModel vm);
    }
}