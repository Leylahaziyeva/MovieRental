using MovieRental.BLL.ViewModels.Movie;

namespace MovieRental.BLL.ViewModels.UserList
{
    public class UserListViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MovieCount { get; set; }
        public IEnumerable<MovieViewModel> Movies { get; set; } = [];
    }
}