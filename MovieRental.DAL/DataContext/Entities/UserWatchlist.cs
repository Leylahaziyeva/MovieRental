namespace MovieRental.DAL.DataContext.Entities
{
    public class UserWatchlist : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public DateTime AddedDate { get; set; }
    }
}