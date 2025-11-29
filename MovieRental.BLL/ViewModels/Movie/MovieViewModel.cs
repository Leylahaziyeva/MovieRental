using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; } 
        public required string PosterImageUrl { get; set; }

        public int Year { get; set; }
        public int Duration { get; set; }
        public decimal RentalPrice { get; set; }

        public int LovePercentage { get; set; }
        public int VotesCount { get; set; }

        public string? Format { get; set; }
        public List<string> Genres { get; set; } = [];

        public bool IsFeatured { get; set; }
        public bool IsAvailableForRent { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? FormattedPrice { get; set; }

        public LanguageViewModel? Language { get; set; }
        public string? LanguageName { get; set; }

        public int Rating => LovePercentage;
        public string PosterUrl => PosterImageUrl;
        public string FormattedDuration => $"{Duration / 60}h {Duration % 60}m";
    }

    public class MovieTranslationViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Plot { get; set; }
        public string? DirectorNames { get; set; }
        public string? WriterNames { get; set; }
        public string? CastNames { get; set; }

        public int MovieId { get; set; }
        public MovieViewModel? Movie { get; set; }

        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}