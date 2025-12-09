//using Microsoft.AspNetCore.Identity;
//using MovieRental.BLL.ViewModels.Account;

//namespace MovieRental.BLL.Services.Contracts
//{
//    public interface IAccountService
//    {
//        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
//        Task<SignInResult> LoginAsync(LoginViewModel model);
//        Task LogoutAsync();
//        Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordViewModel model);
//        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);
//        Task<AccountViewModel?> GetUserProfileAsync(string userId);
//        Task<IdentityResult> UpdateProfileAsync(string userId, AccountViewModel model);
//        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
//        Task<bool> IsEmailExistsAsync(string email);
//        Task<bool> IsUsernameExistsAsync(string username);
//        Task<AccountViewModel?> GetUserByEmailAsync(string email);
//    }
//}


using Microsoft.AspNetCore.Identity;
using MovieRental.BLL.ViewModels.Account;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
        Task<AccountViewModel?> GetUserProfileAsync(string userId);
        Task<IdentityResult> UpdateProfileAsync(string userId, AccountViewModel model);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<AccountViewModel?> GetUserByEmailAsync(string email);
    }
}