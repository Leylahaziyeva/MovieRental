using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Offer
{
    public class OfferCreateViewModel
    {
        public required string Title { get; set; }
        public required string Description { get; set; }

        public required IFormFile ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        public IFormFile? LogoFile { get; set; }
        public string? LogoUrl { get; set; }

        public DateTime ValidFrom { get; set; } = DateTime.Now;
        public DateTime ValidTo { get; set; } = DateTime.Now.AddMonths(1);

        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }

        public int? CurrencyId { get; set; }
        public List<SelectListItem>? CurrencyList { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class OfferTranslationCreateViewModel
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int OfferId { get; set; }
        public OfferViewModel? Offer { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}