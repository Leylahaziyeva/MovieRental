namespace MovieRental.DAL.DataContext.Entities
{
    public class Event : TimeStample
    {
        public required string ImageUrl { get; set; }
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

        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public int? EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public List<EventTranslation> EventTranslations { get; set; } = [];
        public List<Person>? Artists { get; set; }
    }

    public class EventTranslation : TimeStample
    {
        public required string Name { get; set; }
        public required string Description { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}