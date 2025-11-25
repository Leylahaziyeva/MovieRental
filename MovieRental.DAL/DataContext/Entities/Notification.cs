namespace MovieRental.DAL.DataContext.Entities
{
    public class Notification : TimeStample
    {
        //(bell icon with counter)
        public required string UserId { get; set; }
        public AppUser User { get; set; } = null!;
  
        public string? ImageUrl { get; set; }
        public string? ActionUrl { get; set; }
        public string? IconClass { get; set; } // "bg-primary", "bg-success", "bg-warning"

        public bool IsRead { get; set; } = false;
        public DateTime NotificationDate { get; set; }

        public virtual ICollection<NotificationTranslation> NotificationTranslations { get; set; } = new List<NotificationTranslation>();
    }

    public class NotificationTranslation : TimeStample
    {
        public required string Title { get; set; }
        public required string Message { get; set; }

        public int NotificationId { get; set; }
        public virtual Notification Notification { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}