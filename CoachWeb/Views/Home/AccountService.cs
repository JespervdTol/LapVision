using CoachWeb.Services.Interfaces;
using Model.Entities;
using CoachWeb.Infrastructure.Interfaces;
using CoachWeb.Models.Account;

namespace CoachWeb.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Account?> ValidateLoginAsync(string emailOrUsername, string password)
        {
            var account = await _repository.GetByEmailOrUsernameAsync(emailOrUsername);
            if (account == null)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
            return isValid ? account : null;
        }

        public async Task<Account?> RegisterCoachAsync(RegisterCoachViewModel vm)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            return await _repository.CreateCoachAccountAsync(vm, hash);
        }
    }
}