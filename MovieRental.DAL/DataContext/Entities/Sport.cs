namespace MovieRental.DAL.DataContext.Entities
{
    public class Sport : TimeStample
    {
        public required string ImageUrl { get; set; }           
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }        
        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public List<SportTranslation> SportTranslations { get; set; } = new List<SportTranslation>();
    }

    public class SportTranslation : TimeStample
    {
        public required string Name { get; set; }                
        public required string Description { get; set; }         

        public int SportId { get; set; }
        public virtual Sport Sport { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}