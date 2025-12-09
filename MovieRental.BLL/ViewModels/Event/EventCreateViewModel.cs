using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Event
{
    public class EventCreateViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required IFormFile ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        public IFormFile? CoverImageFile { get; set; }
        public string? CoverImageUrl { get; set; }

        public DateTime EventDate { get; set; } = DateTime.Now.AddDays(7);
        public decimal? Price { get; set; }
        public int? CurrencyId { get; set; }
        public List<SelectListItem>? CurrencyList { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Venue { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? AgeRestriction { get; set; }

        public int? EventCategoryId { get; set; }
        public List<SelectListItem>? EventCategoryList { get; set; }

        public int? LocationId { get; set; }
        public List<SelectListItem>? LocationList { get; set; }

        public List<int>? SelectedArtistIds { get; set; }
        public List<SelectListItem>? ArtistList { get; set; }
    }

    public class EventTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int EventId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}