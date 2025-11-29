using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.BLL.ViewModels.Person;

namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }  
        public string? Plot { get; set; }
        public string? DirectorNames { get; set; }  
        public string? WriterNames { get; set; }    
        public string? CastNames { get; set; }

        public required string PosterImageUrl { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string VideoUrl { get; set; }
        public string? TrailerUrl { get; set; }

        public int Year { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal? Budget { get; set; }

        public int LovePercentage { get; set; }
        public int VotesCount { get; set; }

        public decimal RentalPrice { get; set; }
        public int RentalDurationDays { get; set; }
        public bool IsAvailableForRent { get; set; }
        public string? Format { get; set; }

        public CurrencyViewModel? Currency { get; set; }
        public string? FormattedPrice { get; set; }

        public LanguageViewModel? Language { get; set; }
        public string? LanguageName { get; set; }

        public List<GenreViewModel> Genres { get; set; } = [];
        public List<PersonViewModel> Actors { get; set; } = [];
        public List<PersonViewModel> Directors { get; set; } = [];
        public List<PersonViewModel> Writers { get; set; } = [];

        public List<MovieImageViewModel> Images { get; set; } = [];
        public List<MovieVideoViewModel> Videos { get; set; } = [];
        public List<MovieSocialLinkViewModel> SocialLinks { get; set; } = [];
    }
}