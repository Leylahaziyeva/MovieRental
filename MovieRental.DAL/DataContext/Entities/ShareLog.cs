namespace MovieRental.DAL.DataContext.Entities
{
    public class ShareLog : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; } 

        public string? UserId { get; set; }
        public AppUser? User { get; set; }

        public required string Platform { get; set; }  // "Facebook", "Twitter", etc.
        public DateTime ShareDate { get; set; }
        public string? IpAddress { get; set; }
    }
}
