namespace MovieRental.BLL.ViewModels.Offer
{
    public class OfferViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public bool IsActive { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsValid => IsActive && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
        public int DaysRemaining => (ValidTo - DateTime.Now).Days;
    }

    public class OfferTranslationViewModel
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int LanguageId { get; set; }
    }
}