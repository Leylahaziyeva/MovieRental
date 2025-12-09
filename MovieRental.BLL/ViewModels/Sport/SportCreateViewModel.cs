using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportCreateViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public required IFormFile ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? Price { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Venue { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? AgeRestriction { get; set; }

        public int? SportTypeId { get; set; }
        public int? LocationId { get; set; }
        public int? CurrencyId { get; set; }

        public List<SelectListItem>? SportTypeList { get; set; }
        public List<SelectListItem>? LocationList { get; set; }
        public List<SelectListItem>? CurrencyList { get; set; }
    }

    public class SportTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }
        public int SportId { get; set; }
        public SportViewModel? Sport { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? Languages { get; set; }
    }
}