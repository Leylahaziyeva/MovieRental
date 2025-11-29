namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieVideo : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public required string VideoUrl { get; set; }
        public VideoType VideoType { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public List<MovieVideoTranslation> MovieVideoTranslations { get; set; } = [];
    }

    public class MovieVideoTranslation : TimeStample
    {
        public required string Title { get; set; }
        public string? Description { get; set; }

        public int MovieVideoId { get; set; }
        public MovieVideo? MovieVideo { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }

    public enum VideoType
    {
        Trailer = 1,
        Teaser = 2,
        BehindTheScenes = 3,
        Interview = 4,
        FullMovie = 5,
        Clip = 6,
        Other = 7
    }
}
