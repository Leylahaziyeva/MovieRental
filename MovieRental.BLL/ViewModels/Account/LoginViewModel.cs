using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Enter Email Address")]
        public required string Email { get; set; } 

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; } 

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
