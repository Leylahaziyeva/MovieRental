namespace MovieRental.DAL.DataContext.Entities
{
    public class Event : TimeStample
    {
        public required string ImageUrl { get; set; }           
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }            // "Glasgow, Scotland"
        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public List<EventTranslation> EventTranslations { get; set; } = new List<EventTranslation>();
    }

    public class EventTranslation : TimeStample
    {
        public required string Name { get; set; }           
        public required string Description { get; set; }      

        public int EventId { get; set; }
        public virtual Event Event { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}