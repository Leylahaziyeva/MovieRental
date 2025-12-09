using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordMinLength")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "ResetCodeRequired")]
        [Display(Name = "ResetCode")]
        public required string Code { get; set; }
    }
}