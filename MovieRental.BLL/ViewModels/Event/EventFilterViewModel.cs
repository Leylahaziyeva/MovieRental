using MovieRental.BLL.ViewModels.EventCategory;
using MovieRental.BLL.ViewModels.Location;

namespace MovieRental.BLL.ViewModels.Event
{
    public class EventFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public int? LocationId { get; set; }
        public int? EventCategoryId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentLanguageId { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public List<EventCategoryOption> Categories { get; set; } = [];
        public List<LocationOption> Locations { get; set; } = [];
    }
}