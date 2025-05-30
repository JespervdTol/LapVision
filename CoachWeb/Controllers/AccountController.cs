using CoachWeb.Services.Interfaces;
using Contracts.CoachWeb.ViewModels.Account;
using Contracts.CoachWeb.ErrorHandeling;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoachWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.ValidateLoginAsync(model.EmailOrUsername, model.Password);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                TempData["ShowReportButton"] = result.ErrorCategory == ErrorType.SystemError;
                TempData["ErrorContext"] = "Login";
                return RedirectToAction("Login");
            }

            var user = result.Value!;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CoachAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CoachAuth", principal);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterCoachViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterCoachViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _accountService.RegisterCoachAsync(model);
            if (result.IsFailure)
            {
                TempData["ErrorMessage"] = result.Error;
                TempData["ShowReportButton"] = result.ErrorCategory == ErrorType.SystemError;
                TempData["ErrorContext"] = "Register";
                return RedirectToAction("Register");
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CoachAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}