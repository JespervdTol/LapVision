using CoachWeb.Services;
using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ViewModels.Account;
using Infrastructure.CoachWeb.Interfaces;
using Model.Entities.CoachWeb;
using Model.Enums;
using Moq;
using Xunit;

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
        public async Task ValidateLoginAsync_ValidCredentials_ReturnsAccount()
        {
            var email = "test@lapvision.com";
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            var expectedAccount = new Account("testuser", email, hash, UserRole.Coach);

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email)).ReturnsAsync(expectedAccount);

            var result = await _accountService.ValidateLoginAsync(email, password);

            Assert.NotNull(result);
            Assert.Equal(expectedAccount.Email, result!.Email);
        }

        [Fact]
        public async Task ValidateLoginAsync_InvalidPassword_ReturnsNull()
        {
            var email = "test@lapvision.com";
            var wrongPassword = "wrongpassword123";
            var actualPasswordHash = BCrypt.Net.BCrypt.HashPassword("password123");

            var fakeAccount = new Account("user", email, actualPasswordHash, UserRole.Coach);

            _mockRepo.Setup(r => r.GetByEmailOrUsernameAsync(email)).ReturnsAsync(fakeAccount);

            var result = await _accountService.ValidateLoginAsync(email, wrongPassword);

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterCoachAsync_ValidInput_ReturnsAccount()
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

            _mockRepo
                .Setup(r => r.CreateCoachAccountAsync(It.IsAny<Account>(), It.IsAny<Person>()))
                .ReturnsAsync(fakeAccount);

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.NotNull(result);
            Assert.Equal(vm.Email, result!.Email);
        }

        [Fact]
        public async Task RegisterCoachAsync_InvalidInput_ReturnsNull()
        {
            var vm = new RegisterCoachViewModel
            {
                Username = "existingcoach",
                Email = "existing@coach.com",
                Password = "Coach123!",
                FirstName = "Existing",
                Prefix = "",
                LastName = "Coach",
                DateOfBirth = new DateTime(1985, 5, 15)
            };

            _mockRepo
                .Setup(r => r.CreateCoachAccountAsync(It.IsAny<Account>(), It.IsAny<Person>()))
                .ReturnsAsync((Account?)null);

            var result = await _accountService.RegisterCoachAsync(vm);

            Assert.Null(result);
        }
    }
}