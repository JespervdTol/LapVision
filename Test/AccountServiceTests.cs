using CoachWeb.Services;
using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ErrorHandeling;
using Contracts.CoachWeb.ViewModels.Account;
using Infrastructure.CoachWeb.Interfaces;
using Model.Entities.CoachWeb;
using Model.Enums;
using Moq;

namespace Test
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> _mockRepo;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            _mockRepo = new Mock<IAccountRepository>();
            _accountService = new AccountService(_mockRepo.Object);
        }

        [Fact]
        public async Task ValidateLoginAsync_ValidCredentials_ReturnsSuccessResultWithAccount()
        {
            var email = "test@lapvision.com";
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var expectedAccount = new Account("testuser", email, hash, UserRole.Coach);

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email))
                .ReturnsAsync(Result<Account>.Success(expectedAccount));

            var result = await _accountService.ValidateLoginAsync(email, password);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedAccount.Email, result.Value!.Email);
        }

        [Fact]
        public async Task ValidateLoginAsync_InvalidPassword_ReturnsFailureResultWithUserError()
        {
            var email = "test@lapvision.com";
            var wrongPassword = "wrongpassword123";
            var actualPasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");

            var fakeAccount = new Account("user", email, actualPasswordHash, UserRole.Coach);

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email))
                .ReturnsAsync(Result<Account>.Success(fakeAccount));

            var result = await _accountService.ValidateLoginAsync(email, wrongPassword);

            Assert.True(result.IsFailure);
            Assert.Equal("Incorrect password. Please try again.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task ValidateLoginAsync_UserNotFound_ReturnsFailureResultWithUserError()
        {
            var email = "notfound@lapvision.com";

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email))
                .ReturnsAsync(Result<Account>.Failure("User not found.", ErrorType.UserError));

            var result = await _accountService.ValidateLoginAsync(email, "anyPassword");

            Assert.True(result.IsFailure);
            Assert.Equal("User not found.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task ValidateLoginAsync_RepositoryThrowsException_ReturnsFailureResultWithSystemError()
        {
            var email = "error@lapvision.com";

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email))
                .ReturnsAsync(Result<Account>.Failure("Database error", ErrorType.SystemError));

            var result = await _accountService.ValidateLoginAsync(email, "anyPassword");

            Assert.True(result.IsFailure);
            Assert.Equal("Database error", result.Error);
            Assert.Equal(ErrorType.SystemError, result.ErrorCategory);
        }

        [Fact]
        public async Task RegisterCoachAsync_ValidInput_ReturnsSuccessResultWithAccount()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "newcoach",
                Email = "new@coach.com",
                Password = "Coach123!",
                FirstName = "New",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            var hashed = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            var fakeAccount = new Account(vm.Username, vm.Email, hashed, UserRole.Coach);

            _mockRepo.Setup(r => r.EmailExistsAsync(vm.Email))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.UsernameExistsAsync(vm.Username))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.CreateCoachAccountAsync(It.IsAny<Account>(), It.IsAny<Person>()))
                .ReturnsAsync(Result<Account>.Success(fakeAccount));

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(vm.Email, result.Value!.Email);
        }

        [Fact]
        public async Task RegisterCoachAsync_EmailAlreadyExists_ReturnsFailureWithUserError()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "newcoach",
                Email = "existing@coach.com",
                Password = "Coach123!",
                FirstName = "New",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(vm.Email))
                .ReturnsAsync(true);

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.True(result.IsFailure);
            Assert.Equal("This email is already registered. Try logging in or use a different email.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task RegisterCoachAsync_UsernameAlreadyExists_ReturnsFailureWithUserError()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "existingcoach",
                Email = "new@coach.com",
                Password = "Coach123!",
                FirstName = "New",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(vm.Email))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.UsernameExistsAsync(vm.Username))
                .ReturnsAsync(true);

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.True(result.IsFailure);
            Assert.Equal("Username is already taken. Please choose another one.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task RegisterCoachAsync_PasswordTooShort_ReturnsFailureWithUserError()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "newcoach",
                Email = "new@coach.com",
                Password = "123",
                FirstName = "New",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(vm.Email))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.UsernameExistsAsync(vm.Username))
                .ReturnsAsync(false);

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.True(result.IsFailure);
            Assert.Equal("Password must be at least 6 characters long.", result.Error);
            Assert.Equal(ErrorType.UserError, result.ErrorCategory);
        }

        [Fact]
        public async Task RegisterCoachAsync_CreateCoachAccountFails_ReturnsFailureWithSystemError()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "newcoach",
                Email = "new@coach.com",
                Password = "Coach123!",
                FirstName = "New",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            _mockRepo.Setup(r => r.EmailExistsAsync(vm.Email))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.UsernameExistsAsync(vm.Username))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.CreateCoachAccountAsync(It.IsAny<Account>(), It.IsAny<Person>()))
                .ReturnsAsync(Result<Account>.Failure("DB insert error", ErrorType.SystemError));

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.True(result.IsFailure);
            Assert.Equal("DB insert error", result.Error);
            Assert.Equal(ErrorType.SystemError, result.ErrorCategory);
        }
    }
}