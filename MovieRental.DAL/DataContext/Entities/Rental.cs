namespace MovieRental.DAL.DataContext.Entities
{
    public class Rental : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public DateTime RentalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }

        public RentalStatus Status { get; set; } = RentalStatus.Active;

        // Payment tracking (Stripe üçün hazırlıq)
        public string? PaymentIntentId { get; set; }  
        public string? TransactionId { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }

        //public string? StripeCustomerId { get; set; }
        //public string? StripePaymentMethodId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool HasWatched { get; set; }

        public DateTime? FirstWatchedAt { get; set; }
        public DateTime? LastWatchedAt { get; set; }
        public int TotalWatchTimeSeconds { get; set; }

        public List<WatchHistory> WatchHistories { get; set; } = [];

        public bool IsExpired => DateTime.UtcNow > ExpiryDate;
        public TimeSpan TimeRemaining => ExpiryDate - DateTime.UtcNow;
        public int HoursRemaining => (int)TimeRemaining.TotalHours;
        public int DaysRemaining => (int)TimeRemaining.TotalDays;
    }
}