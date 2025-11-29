namespace MovieRental.BLL.ViewModels.Event
{
    public class EventFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public string? Location { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Category { get; set; }
        public int CurrentLanguageId { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}