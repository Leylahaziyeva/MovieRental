namespace MovieRental.BLL.ViewModels.Event
{
    public class EventUpdateViewModel
    {
        public required string ImageUrl { get; set; }
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }
        public decimal? Price { get; set; }
        public int? CurrencyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }

        public List<EventTranslationViewModel> Translations { get; set; } = new();
    }
}