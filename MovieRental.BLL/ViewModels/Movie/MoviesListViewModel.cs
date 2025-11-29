namespace MovieRental.BLL.ViewModels.Movie
{
    public class MoviesListViewModel
    {
        public IEnumerable<MovieViewModel> Movies { get; set; } = [];
        public MovieFilterViewModel Filter { get; set; } = new();
        public int TotalCount { get; set; }
        public string PageTitle { get; set; } = "All Movies";

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)Filter.PageSize);
        public bool HasPreviousPage => Filter.Page > 1;
        public bool HasNextPage => Filter.Page < TotalPages;
    }
}