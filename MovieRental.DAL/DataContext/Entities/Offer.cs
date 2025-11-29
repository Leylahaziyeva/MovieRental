namespace MovieRental.DAL.DataContext.Entities
{
    public class Offer : TimeStample
    {
        //(fire icon with "NEW" badge)
        public required string ImageUrl { get; set; }       
        public required string LogoUrl { get; set; }      // Bank logosu
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool IsActive { get; set; } = true;

        public decimal? DiscountAmount { get; set; }
        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public List<OfferTranslation> OfferTranslations { get; set; } = [];
    }

    public class OfferTranslation : TimeStample
    {
        public required string Title { get; set; }            
        public required string Description { get; set; }    

        public int OfferId { get; set; }
        public Offer? Offer { get; set; } 

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}