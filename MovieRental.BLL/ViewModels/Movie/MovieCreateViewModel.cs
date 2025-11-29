using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieCreateViewModel
    {
        public required string Title { get; set; }
        public required string Plot { get; set; }
        public required string DirectorNames { get; set; }  
        public required string WriterNames { get; set; }    
        public required string CastNames { get; set; }

        public required IFormFile PosterImage { get; set; }
        public required IFormFile CoverImage { get; set; }
        public required IFormFile Video { get; set; }
        public string? TrailerUrl { get; set; }

        public int Year { get; set; }
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; } = DateTime.Now.AddMonths(3);
        public decimal? Budget { get; set; }

        public decimal RentalPrice { get; set; }
        public int RentalDurationDays { get; set; } = 3;
        public bool IsAvailableForRent { get; set; } = true;

        public string? Format { get; set; }
        public bool IsFeatured { get; set; }

        public int LanguageId { get; set; }
        public int CurrencyId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
        public List<SelectListItem>? CurrencyList { get; set; }

        public List<int>? SelectedGenreIds { get; set; }
        public List<SelectListItem>? GenreList { get; set; }

        public List<int>? SelectedActorIds { get; set; }
        public List<SelectListItem>? ActorList { get; set; }

        public List<int>? SelectedDirectorIds { get; set; }
        public List<SelectListItem>? DirectorList { get; set; }

        public List<int>? SelectedWriterIds { get; set; }
        public List<SelectListItem>? WriterList { get; set; }

        public List<IFormFile>? AdditionalImages { get; set; }
        public List<MovieVideoCreateDto>? AdditionalVideos { get; set; }
        public List<MovieSocialLinkCreateDto>? SocialLinks { get; set; }
    }

    public class MovieTranslationCreateViewModel
    {
        public required string Title { get; set; }
        public required string Plot { get; set; }
        public required string DirectorNames { get; set; }
        public required string WriterNames { get; set; }
        public required string CastNames { get; set; }

        public int MovieId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}