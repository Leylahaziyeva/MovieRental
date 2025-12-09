namespace MovieRental.DAL.DataContext.Entities
{
    public class UserList : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }

        public ICollection<UserListMovie>? UserListMovies { get; set; }
    }
}