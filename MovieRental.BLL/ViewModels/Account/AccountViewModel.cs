using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class AccountViewModel
    {
        [Display(Name = "Username")]
        public required string UserName { get; set; } 

        [Display(Name = "First Name")]
        public required string FirstName { get; set; } 

        [Display(Name = "Last Name")]
        public required string LastName { get; set; } 

        [Display(Name = "Email Address")]
        [EmailAddress]
        public required string Email { get; set; } 
    }
}
