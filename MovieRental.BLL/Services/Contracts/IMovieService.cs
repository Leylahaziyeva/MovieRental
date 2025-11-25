using MovieRental.BLL.ViewModels.Movie;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IMovieService
    {
        Task<MoviesListViewModel> GetFilteredMoviesAsync(MovieFilterViewModel filter);
        Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int movieId, int languageId);
        Task<IEnumerable<MovieCardViewModel>> GetFeaturedMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieCardViewModel>> GetLatestMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieCardViewModel>> GetPopularMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieCardViewModel>> GetUpcomingMoviesAsync(int languageId, int count = 10);
    }
}