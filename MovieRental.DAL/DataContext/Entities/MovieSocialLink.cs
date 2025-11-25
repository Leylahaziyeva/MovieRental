namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieSocialLink : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public required string Platform { get; set; }  // "Facebook", "Twitter", "YouTube", "Instagram", "Snapchat"
        public required string Url { get; set; }       // Full URL to social media page
        public string? IconClass { get; set; }         // "fab fa-facebook-f", "fab fa-twitter"

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
