using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.ViewModels.Rental
{
    public class RentalResultViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? RentalId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? Price { get; set; }
    }
    public class RentalViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int RentalDurationDays { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string MoviePosterUrl { get; set; } = string.Empty;
        public string MovieLanguage { get; set; } = string.Empty;

        public DateTime RentalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;

        public RentalStatus Status { get; set; }
        public bool IsExpired { get; set; }
        public bool HasWatched { get; set; }

        public int DaysRemaining { get; set; }
        public int HoursRemaining { get; set; }
        public string TimeRemainingText { get; set; } = string.Empty;

        public DateTime? FirstWatchedAt { get; set; }
        public DateTime? LastWatchedAt { get; set; }
        public int TotalWatchTimeMinutes { get; set; }
    }

    public class WatchHistoryViewModel
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string MoviePosterUrl { get; set; } = string.Empty;

        public DateTime WatchedAt { get; set; }
        public int WatchDurationMinutes { get; set; }
        public decimal CompletionPercentage { get; set; }
        public bool IsCompleted { get; set; }

        public string DeviceType { get; set; } = string.Empty;
        public bool WasRented { get; set; }
    }

    public class RentMovieRequest
    {
        public int MovieId { get; set; }
        public string? PaymentMethodId { get; set; }  // Stripe üçün
    }
}