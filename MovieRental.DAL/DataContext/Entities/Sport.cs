namespace MovieRental.DAL.DataContext.Entities
{
    public class Sport : TimeStample
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

        public int? SportTypeId { get; set; }
        public SportType? SportType { get; set; }

        public int? LocationId { get; set; }
        public Location? Location { get; set; }

        public List<SportTranslation> SportTranslations { get; set; } = [];
        public List<Person> Players { get; set; } = [];
    }

    public class SportTranslation : TimeStample
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Location { get; set; }

        public int SportId { get; set; }
        public Sport? Sport { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}