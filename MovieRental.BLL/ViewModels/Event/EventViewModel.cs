namespace MovieRental.BLL.ViewModels.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int DaysUntilEvent => (EventDate - DateTime.Now).Days;
        public bool IsUpcoming => EventDate > DateTime.Now;
    }

    public class EventTranslationViewModel
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int LanguageId { get; set; }
    }
}