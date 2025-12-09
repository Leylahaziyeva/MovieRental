using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UsernameLengthError")]
        [Display(Name = "Username")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "FirstNameRequired")]
        [Display(Name = "FirstName")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        [Display(Name = "LastName")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordMinLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        [Display(Name = "ConfirmPassword")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "YouMustAgreeToTerms")]
        public bool AgreeToTerms { get; set; }
    }
}
