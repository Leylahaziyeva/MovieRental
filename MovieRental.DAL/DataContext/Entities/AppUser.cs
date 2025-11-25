using Microsoft.AspNetCore.Identity;

namespace MovieRental.DAL.DataContext.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public virtual List<Review> Reviews { get; set; } = new List<Review>();
        public virtual List<UserWatchlist> Watchlists { get; set; } = new List<UserWatchlist>();
        public virtual List<Rental> Rentals { get; set; } = new List<Rental>();
        public virtual List<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();
        public virtual List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}