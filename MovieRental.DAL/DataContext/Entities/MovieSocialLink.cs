namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieSocialLink : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public required string Platform { get; set; }  // "Facebook", "Twitter", "YouTube", "Instagram", "Snapchat"
        public required string Url { get; set; }       
        public string? IconClass { get; set; }         // "fab fa-facebook-f", "fab fa-twitter"

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
