using System.ComponentModel.DataAnnotations;

namespace MovieRental.BLL.ViewModels.UserList
{
    public class CreateUserListViewModel
    {
        [Required(ErrorMessage = "List name is required")]
        [StringLength(200, ErrorMessage = "List name cannot exceed 200 characters")]
        public required string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
