namespace MovieRental.DAL.DataContext.Entities
{
    public class Offer : TimeStample
    {
        //(fire icon with "NEW" badge on header nav)
        public required string ImageUrl { get; set; }       
        public string? LogoUrl { get; set; }    
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }

        public bool IsActive { get; set; } = true;

        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public List<OfferTranslation> OfferTranslations { get; set; } = [];

        public bool IsValid => IsActive && ValidFrom <= DateTime.Now && ValidTo >= DateTime.Now;
        public bool IsExpired => DateTime.Now > ValidTo;
        public bool IsUpcoming => DateTime.Now < ValidFrom;
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