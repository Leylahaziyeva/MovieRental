using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.Offer
{
    public class OfferViewModel
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; } 
        public string? LogoUrl { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool IsActive { get; set; }

        public CurrencyViewModel? Currency { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        public bool IsValid => IsActive && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
        public int DaysRemaining => (ValidTo - DateTime.Now).Days;
    }

    public class OfferTranslationViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}