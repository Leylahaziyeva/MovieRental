
namespace MovieRental.DAL.DataContext.Entities
{
    public class WatchHistory : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int? RentalId { get; set; }
        public Rental? Rental { get; set; }

        public DateTime WatchedAt { get; set; }
        public int WatchDurationSeconds { get; set; }
        public int TotalDurationSeconds { get; set; }
        public decimal CompletionPercentage { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
    }

    public enum RentalStatus
    {
        Active,
        Expired,
        Watched,
        Cancelled
    }
}