using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Account;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class AccountManager : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountManager> _logger;

        public AccountManager(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            ILogger<AccountManager> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "DuplicateEmail",
                        Description = "Email already exists"
                    });
                }

                var existingUserByUsername = await _userManager.FindByNameAsync(model.Username);
                if (existingUserByUsername != null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "DuplicateUserName",
                        Description = "Username already exists"
                    });
                }

                var user = new AppUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailConfirmed = false, 
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Username} registered successfully");

                    // TODO: Send confirmation email
                    // TODO: Add to default role (User)
                    // await _userManager.AddToRoleAsync(user, "User");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RegistrationError",
                    Description = "An error occurred during registration"
                });
            }
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Login attempt failed: User not found - {model.Email}");
                    return SignInResult.Failed;
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning($"Login attempt failed: User is not active - {model.Email}");
                    return SignInResult.NotAllowed;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true 
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {model.Email} logged in successfully");
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning($"User account locked out - {model.Email}");
                }
                else if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation($"User requires two-factor authentication - {model.Email}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return SignInResult.Failed;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }

        public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return IdentityResult.Success;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // TODO: Send email with reset link
                // Example: https://yoursite.com/account/reset-password?email={email}&token={token}

                _logger.LogInformation($"Password reset token generated for {model.Email}");

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ForgotPasswordError",
                    Description = "An error occurred while processing your request"
                });
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found"
                    });
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password reset successfully for {model.Email}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ResetPasswordError",
                    Description = "An error occurred while resetting your password"
                });
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found"
                    });
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password changed successfully for user {userId}");

                    await _signInManager.RefreshSignInAsync(user);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ChangePasswordError",
                    Description = "An error occurred while changing your password"
                });
            }
        }

        public async Task<AccountViewModel?> GetUserProfileAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return null;

                return new AccountViewModel
                {
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return null;
            }
        }

        public async Task<IdentityResult> UpdateProfileAsync(string userId, AccountViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "UserNotFound",
                        Description = "User not found"
                    });
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Profile updated successfully for user {userId}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UpdateProfileError",
                    Description = "An error occurred while updating your profile"
                });
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null;
        }

        public async Task<AccountViewModel?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                    return null;

                return new AccountViewModel
                {
                    UserName = user.UserName!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email");
                return null;
            }
        }
    }
}