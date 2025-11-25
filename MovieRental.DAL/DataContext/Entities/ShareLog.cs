namespace MovieRental.DAL.DataContext.Entities
{
    public class ShareLog : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }

        public required string Platform { get; set; }  // "Facebook", "Twitter", etc.
        public DateTime ShareDate { get; set; }
        public string? IpAddress { get; set; }
    }
}
