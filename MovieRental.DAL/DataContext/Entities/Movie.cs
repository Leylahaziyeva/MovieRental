namespace MovieRental.DAL.DataContext.Entities
{
    public class Movie : TimeStample
    {
        public required string PosterImageUrl { get; set; }      // Cloudinary
        public required string CoverImageUrl { get; set; }       // Cloudinary
        public required string VideoUrl { get; set; }            // Cloudinary Video
        public string? TrailerUrl { get; set; }                  // YouTube URL

        public int Year { get; set; }
        public int Duration { get; set; }                        // Dəqiqə
        public DateTime ReleaseDate { get; set; }
        public decimal? Budget { get; set; }
        public int LovePercentage { get; set; }                  // 88%
        public int VotesCount { get; set; }                      // 23,421

        public decimal RentalPrice { get; set; }
        public int RentalDurationDays { get; set; }
        public bool IsAvailableForRent { get; set; }

        public string? Format { get; set; }                     // "2D", "3D", "5D"

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public int LanguageId { get; set; }                     
        public  Language Language { get; set; } = null!;

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public List<MovieTranslation> MovieTranslations { get; set; } = new List<MovieTranslation>();
        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public List<MovieImage> MovieImages { get; set; } = new List<MovieImage>();
        public List<MovieVideo> MovieVideos { get; set; } = new List<MovieVideo>(); 
        public List<MovieSocialLink> MovieSocialLinks { get; set; } = new List<MovieSocialLink>();  
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<UserWatchlist> UserWatchlists { get; set; } = new List<UserWatchlist>();
        public List<Rental> Rentals { get; set; } = new List<Rental>();
        public List<CollectionMovie> CollectionMovies { get; set; } = new List<CollectionMovie>();
}

    public class MovieTranslation : TimeStample
    {
        public required string Title { get; set; }
        public required string Plot { get; set; }                // Süjet 
        public required string Director { get; set; }            // Rejissor 
        public required string Writers { get; set; }             // Yazıçı
        public required string Cast { get; set; }                // Aktyor heyəti

        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}