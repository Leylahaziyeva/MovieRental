namespace MovieRental.BLL.ViewModels.Offer
{
    public class OfferCreateViewModel
    {
        public required string ImageUrl { get; set; }
        public required string LogoUrl { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? CurrencyId { get; set; }
        public bool IsActive { get; set; } = true;

        public List<OfferTranslationViewModel> Translations { get; set; } = new();
    }
}