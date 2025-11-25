namespace MovieRental.BLL.ViewModels.Event
{
    public class EventFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public string? Location { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentLanguageId { get; set; }
    }
}