namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieFilterViewModel
    {
        public string? Genre { get; set; }
        public string? Language { get; set; }
        public string? Sort { get; set; }
        public int? Year { get; set; }
        public string? Format { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CurrentLanguageId { get; set; }
        public string? SearchQuery { get; set; }
    }
}
