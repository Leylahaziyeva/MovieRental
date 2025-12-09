namespace MovieRental.DAL.DataContext.Entities
{
    public class UserListMovie : TimeStample
    {
        public int UserListId { get; set; }
        public UserList? UserList { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public DateTime AddedDate { get; set; }
    }
}