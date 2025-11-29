namespace MovieRental.DAL.DataContext.Entities
{
    public class Notification : TimeStample
    {
        //(bell icon with counter)
        public required string UserId { get; set; }
        public AppUser? User { get; set; }
  
        public string? ImageUrl { get; set; }
        public string? ActionUrl { get; set; }
        public string? IconClass { get; set; } // "bg-primary", "bg-success", "bg-warning"

        public bool IsRead { get; set; } = false;
        public DateTime NotificationDate { get; set; }

        public virtual ICollection<NotificationTranslation> NotificationTranslations { get; set; } = [];
    }

    public class NotificationTranslation : TimeStample
    {
        public required string Title { get; set; }
        public required string Message { get; set; }

        public int NotificationId { get; set; }
        public Notification? Notification { get; set; } 

        public int LanguageId { get; set; }
        public Language? Language { get; set; } 
    }
}