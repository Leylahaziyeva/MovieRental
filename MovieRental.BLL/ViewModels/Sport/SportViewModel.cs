using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.BLL.ViewModels.Person;

namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? Price { get; set; }
        public string? FormattedPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? Venue { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? AgeRestriction { get; set; }

        public string? Categories { get; set; }

        public string? Languages { get; set; }

        public CurrencyViewModel? Currency { get; set; }

        public int DaysUntilEvent => (EventDate - DateTime.Now).Days;
        public bool IsUpcoming => EventDate > DateTime.Now;
        public string FormattedDate => EventDate.ToString("ddd, MMM dd, yyyy");
        public string FormattedTime => EventDate.ToString("hh:mm tt");
    }

    public class SportTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public int SportId { get; set; }
        public SportViewModel? Sport { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}