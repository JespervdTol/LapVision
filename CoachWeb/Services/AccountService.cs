using BCrypt.Net;
using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.ViewModels.Account;
using Infrastructure.CoachWeb.Interfaces;
using Model.Entities.CoachWeb;
using Model.Enums;

namespace CoachWeb.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Account>> ValidateLoginAsync(string emailOrUsername, string password)
        {
            var result = await _repository.GetByEmailOrUsernameAsync(emailOrUsername);

            if (result.IsFailure)
                return result;

            var account = result.Value!;
            if (!BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
                return Result<Account>.Failure("Incorrect password. Please try again.", ErrorType.UserError);

            return Result<Account>.Success(account);
        }

        public async Task<Result<Account>> RegisterCoachAsync(RegisterCoachViewModel vm)
        {
            if (await _repository.EmailExistsAsync(vm.Email))
                return Result<Account>.Failure("This email is already registered. Try logging in or use a different email.", ErrorType.UserError);

            if (await _repository.UsernameExistsAsync(vm.Username))
                return Result<Account>.Failure("Username is already taken. Please choose another one.", ErrorType.UserError);

            if (vm.Password.Length < 6)
                return Result<Account>.Failure("Password must be at least 6 characters long.", ErrorType.UserError);

            var hash = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            var dob = DateOnly.FromDateTime(vm.DateOfBirth);

            var account = new Account(vm.Username, vm.Email, hash, UserRole.Coach);
            var person = new Person(vm.FirstName, vm.Prefix, vm.LastName, dob);

            person.SetAccount(account);
            account.SetPerson(person);

            var result = await _repository.CreateCoachAccountAsync(account, person);
            if (result.IsFailure)
                return Result<Account>.Failure(result.Error!, result.ErrorCategory ?? ErrorType.SystemError);

            return Result<Account>.Success(account);
        }
    }
}
