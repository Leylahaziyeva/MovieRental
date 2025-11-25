namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieVideo : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public required string VideoUrl { get; set; }  // YouTube embed URL
        public string? VideoType { get; set; }         // "Trailer", "Teaser", "Behind the Scenes", "Interview"
        public string? ThumbnailUrl { get; set; }      // Video thumbnail

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual List<MovieVideoTranslation> MovieVideoTranslations { get; set; } = new List<MovieVideoTranslation>();
    }

    public class MovieVideoTranslation : TimeStample
    {
        public required string Title { get; set; }     
        public string? Description { get; set; }       

        public int MovieVideoId { get; set; }
        public virtual MovieVideo MovieVideo { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}