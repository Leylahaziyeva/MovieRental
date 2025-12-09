using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "CurrentPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "NewPasswordRequired")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordMinLength")]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword")]
        [Compare("NewPassword", ErrorMessage = "PasswordsDoNotMatch")]
        public required string ConfirmPassword { get; set; }
    }
}