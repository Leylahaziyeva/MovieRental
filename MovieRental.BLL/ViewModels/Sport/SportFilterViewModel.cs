namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public string? Location { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentLanguageId { get; set; }
    }
}