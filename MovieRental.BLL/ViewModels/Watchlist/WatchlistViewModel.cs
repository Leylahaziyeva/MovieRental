using MovieRental.BLL.ViewModels.Movie;

namespace MovieRental.BLL.ViewModels.Watchlist
{
    public class WatchlistViewModel
    {
        public IEnumerable<MovieViewModel> Movies { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}