namespace MovieRental.DAL.DataContext.Entities
{
    public class Movie : TimeStample
    {
        public required string PosterImageUrl { get; set; }      
        public required string CoverImageUrl { get; set; }      
        public required string VideoUrl { get; set; }           
        public string? TrailerUrl { get; set; }                 

        public int Year { get; set; }
        public int Duration { get; set; }    // Dəqiqə
        public DateTime ReleaseDate { get; set; }
        public decimal? Budget { get; set; }

        public int LovePercentage { get; set; }                  
        public int VotesCount { get; set; }                    

        public decimal RentalPrice { get; set; }
        public int RentalDurationDays { get; set; }
        public bool IsAvailableForRent { get; set; }

        public string? Format { get; set; }                 
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;

        public List<MovieTranslation> MovieTranslations { get; set; } = [];
        public List<MovieGenre> MovieGenres { get; set; } = [];
        public List<MoviePerson> MoviePersons { get; set; } = [];  
        public List<MovieImage> MovieImages { get; set; } = [];
        public List<MovieVideo> MovieVideos { get; set; } = [];
        public List<MovieSocialLink> MovieSocialLinks { get; set; } = [];
        public List<Review> Reviews { get; set; } = [];
        public List<UserWatchlist> UserWatchlists { get; set; } = [];
        public List<Rental> Rentals { get; set; } = [];
    }

    public class MovieTranslation : TimeStample
    {
        public required string Title { get; set; }
        public required string Plot { get; set; }
        public required string Director { get; set; }
        public required string Writers { get; set; }
        public required string Cast { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}