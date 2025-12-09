using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "EmailRequired")]  
        [EmailAddress(ErrorMessage = "InvalidEmailFormat")] 
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]  
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
