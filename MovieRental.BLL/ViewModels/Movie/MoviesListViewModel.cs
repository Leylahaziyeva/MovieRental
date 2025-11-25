namespace MovieRental.BLL.ViewModels.Movie
{
    public class MoviesListViewModel
    {
        public IEnumerable<MovieCardViewModel> Movies { get; set; } = [];
        public MovieFilterViewModel Filter { get; set; } = new();
        public int TotalCount { get; set; }
        public string PageTitle { get; set; } = "All Movies";
    }
}
