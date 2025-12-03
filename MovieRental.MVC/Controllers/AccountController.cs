using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.Services.Implementations;
using MovieRental.BLL.ViewModels.Account;
using System.Security.Claims;

namespace MovieRental.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly StringLocalizerManager _localizer;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountService accountService,
            StringLocalizerManager localizer,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _localizer = localizer;
            _logger = logger;
        }

        #region Register

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("RegistrationSuccessful");
                _logger.LogInformation($"User {model.Username} registered successfully");
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion

        #region Login

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.LoginAsync(model);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {model.Email} logged in successfully");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User account locked out - {model.Email}");
                ModelState.AddModelError(string.Empty, _localizer.GetValue("AccountLockedOut"));
                return View(model);
            }

            if (result.RequiresTwoFactor)
            {
                // TODO: Redirect to two-factor authentication page
                return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
            }

            ModelState.AddModelError(string.Empty, _localizer.GetValue("InvalidLoginAttempt"));
            return View(model);
        }

        #endregion

        #region Logout

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            _logger.LogInformation("User logged out");
            TempData["Success"] = _localizer.GetValue("LogoutSuccessful");
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Forgot Password

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.ForgotPasswordAsync(model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("PasswordResetEmailSent");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Reset Password

        [HttpGet]
        public IActionResult ResetPassword(string? code = null, string? email = null)
        {
            if (code == null || email == null)
            {
                return BadRequest("A code and email must be supplied for password reset.");
            }

            var model = new ResetPasswordViewModel
            {
                Code = code,
                Email = email,
                Password = string.Empty,
                ConfirmPassword = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.ResetPasswordAsync(model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("PasswordResetSuccessful");
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Change Password

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _accountService.ChangePasswordAsync(userId, model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("PasswordChangedSuccessfully");
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion

        #region Profile

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var profile = await _accountService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login));
            }

            var result = await _accountService.UpdateProfileAsync(userId, model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("ProfileUpdatedSuccessfully");
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Profile", model);
        }

        #endregion

        #region Helper Methods

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            // TODO: Implement two-factor authentication (Optional - for future use)
            return View();
        }

        #endregion
    }
}