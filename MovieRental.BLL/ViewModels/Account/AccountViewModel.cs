using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.Account
{
    public class AccountViewModel
    {
        public IFormFile? ProfileImageFile { get; set; }
        public string? ProfileImagePath { get; set; }

        [Required(ErrorMessage = "UsernameRequired")]
        [Display(Name = "Username")]
        public required string UserName { get; set; }

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
        public string? Company { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}