using AutoMapper;
using Mailing;
using Microsoft.AspNetCore.Http;
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
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountManager(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            ILogger<AccountManager> logger,
            IMailService mailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
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

                    // Welcome email göndər
                    try
                    {
                        var mail = new Mail
                        {
                            ToEmail = user.Email!,
                            ToFullName = $"{user.FirstName} {user.LastName}",
                            Subject = "Xoş gəlmisiniz - MovieRental",
                            HtmlBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                                    <h2 style='color: #333;'>Xoş gəldiniz {user.FirstName}!</h2>
                                    <p>MovieRental-da qeydiyyatdan keçdiyiniz üçün təşəkkür edirik.</p>
                                    <p>Hesabınız uğurla yaradıldı.</p>
                                    <p><strong>İstifadəçi adı:</strong> {user.UserName}</p>
                                    <p>Hörmətlə,<br>MovieRental Komandası</p>
                                </div>
                            ",
                            TextBody = $"Xoş gəldiniz {user.FirstName}! MovieRental-da qeydiyyatdan keçdiyiniz üçün təşəkkür edirik."
                        };

                        _mailService.SendMail(mail);
                        _logger.LogInformation($"Welcome email sent to {user.Email}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send welcome email");
                    }
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
                return Microsoft.AspNetCore.Identity.SignInResult.Failed;
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

                // Reset linki
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request != null)
                {
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    var resetLink = $"{baseUrl}/account/resetpassword?email={Uri.EscapeDataString(model.Email)}&code={Uri.EscapeDataString(token)}";

                    try
                    {
                        var mail = new Mail
                        {
                            ToEmail = user.Email!,
                            ToFullName = $"{user.FirstName} {user.LastName}",
                            Subject = "Şifrə sıfırlama sorğusu - MovieRental",
                            HtmlBody = $@"
                                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                                    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                                        <h1 style='color: white; margin: 0;'>MovieRental</h1>
                                    </div>
                                    <div style='background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 10px 10px;'>
                                        <h2 style='color: #333; margin-top: 0;'>Şifrənizi sıfırlayın</h2>
                                        <p style='color: #666; line-height: 1.6;'>Salam <strong>{user.FirstName}</strong>,</p>
                                        <p style='color: #666; line-height: 1.6;'>Şifrənizi sıfırlamaq üçün sorğu göndərdiniz. Aşağıdakı düyməyə klikləyərək yeni şifrə təyin edə bilərsiniz:</p>
                                        <div style='text-align: center; margin: 30px 0;'>
                                            <a href='{resetLink}' style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 14px 40px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;'>Şifrəni sıfırla</a>
                                        </div>
                                        <p style='color: #999; font-size: 14px;'>Əgər bu sorğunu siz göndərməmisinizsə, bu emaili nəzərə almayın.</p>
                                        <p style='color: #999; font-size: 14px;'>Bu link 24 saat ərzində etibarlıdır.</p>
                                        <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 30px 0;'>
                                        <p style='color: #999; font-size: 12px; text-align: center;'>© 2024 MovieRental</p>
                                    </div>
                                </div>
                            ",
                            TextBody = $"Şifrə sıfırlama linki: {resetLink}"
                        };

                        _mailService.SendMail(mail);
                        _logger.LogInformation($"Password reset email sent to {user.Email}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send password reset email");
                    }
                }

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
                    Email = user.Email!,
                    Company = user.Company,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    ProfileImagePath = user.ProfileImage
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
                user.Company = model.Company;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;

                if (!string.IsNullOrEmpty(model.ProfileImagePath))
                {
                    user.ProfileImage = model.ProfileImagePath;
                }

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