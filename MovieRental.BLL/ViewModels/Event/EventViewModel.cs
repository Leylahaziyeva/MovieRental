using MovieRental.BLL.ViewModels.Language;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.BLL.ViewModels.Currency;

namespace MovieRental.BLL.ViewModels.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public required string ImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? Price { get; set; }
        public string? FormattedPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        public CurrencyViewModel? Currency { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Venue { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? AgeRestriction { get; set; }

        // Related Entities
        public string? CategoryName { get; set; }
        public string? LocationName { get; set; }
        public List<PersonViewModel>? Artists { get; set; }

        public int DaysUntilEvent => (EventDate - DateTime.Now).Days;
        public bool IsUpcoming => EventDate > DateTime.Now;
        public string FormattedDate => EventDate.ToString("ddd, MMM dd, yyyy");
        public string FormattedTime => EventDate.ToString("hh:mm tt");
        public string EventDay => EventDate.ToString("dd");
        public string EventMonth => EventDate.ToString("MMM").ToUpper();
    }

    public class EventTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int EventId { get; set; }
        public EventViewModel? Event { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}