namespace MovieRental.DAL.DataContext.Entities
{
    public class UserWatchlist : TimeStample
    {
        public required string UserId { get; set; }
        public virtual AppUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public DateTime AddedDate { get; set; }
    }
}