using MovieRental.BLL.ViewModels.Actor;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.MovieImage;
using MovieRental.BLL.ViewModels.MovieSocialLink;
using MovieRental.BLL.ViewModels.MovieVideo;

namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Writers { get; set; } = string.Empty;
        public string Cast { get; set; } = string.Empty;

        public string PosterImageUrl { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
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

        public List<GenreViewModel> Genres { get; set; } = [];
        public List<ActorViewModel> Actors { get; set; } = [];
        public List<MovieImageViewModel> Images { get; set; } = [];
        public List<MovieVideoViewModel> Videos { get; set; } = [];
        public List<MovieSocialLinkViewModel> SocialLinks { get; set; } = [];
    }
}
