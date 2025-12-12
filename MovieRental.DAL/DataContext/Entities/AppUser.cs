using Microsoft.AspNetCore.Identity;

namespace MovieRental.DAL.DataContext.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public string? Company { get; set; }
        public string? Address { get; set; }

        public string? ProfileImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual List<Review> Reviews { get; set; } = [];
        public virtual List<UserWatchlist> Watchlists { get; set; } = [];
        public virtual List<WatchHistory> WatchHistories { get; set; } = [];
        public virtual List<Rental> Rentals { get; set; } = [];
        public virtual List<SearchHistory> SearchHistories { get; set; } = [];
        public virtual List<Notification> Notifications { get; set; } = [];
    }
}