using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ViewModels.Account;
using Infrastructure.CoachWeb.Interfaces;
using Model.Entities.CoachWeb;
using Model.Enums;
using BCrypt.Net;

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
            if (account == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, account.PasswordHash);
            return isValid ? account : null;
        }

        public async Task<Account?> RegisterCoachAsync(RegisterCoachViewModel vm)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            var dob = DateOnly.FromDateTime(vm.DateOfBirth);

            var account = new Account(vm.Username, vm.Email, hash, UserRole.Coach);
            var person = new Person(vm.FirstName, vm.Prefix, vm.LastName, dob);

            person.SetAccount(account);
            account.SetPerson(person);

            return await _repository.CreateCoachAccountAsync(account, person);
        }
    }
}