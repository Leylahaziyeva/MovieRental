using MovieRental.BLL.ViewModels.Movie;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IMovieService : ICrudService<Movie, MovieViewModel, MovieCreateViewModel, MovieUpdateViewModel>
    {
        Task<MoviesListViewModel> GetFilteredMoviesAsync(MovieFilterViewModel filter);
        Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int movieId, int languageId);

        Task<IEnumerable<MovieViewModel>> GetFeaturedMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieViewModel>> GetLatestMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync(int languageId, int count = 10);
        Task<IEnumerable<MovieViewModel>> GetUpcomingMoviesAsync(int languageId, int count = 10);

        Task<bool> AddActorToMovieAsync(int movieId, int personId, string? characterName = null, int displayOrder = 0);
        Task<bool> AddDirectorToMovieAsync(int movieId, int personId);
        Task<bool> AddWriterToMovieAsync(int movieId, int personId);
        Task<bool> RemovePersonFromMovieAsync(int movieId, int personId, MoviePersonRole role);

        Task<bool> AddGenreToMovieAsync(int movieId, int genreId);
        Task<bool> RemoveGenreFromMovieAsync(int movieId, int genreId);
        Task<List<int>> GetMovieGenreIdsAsync(int movieId);

        Task<bool> AddMovieImageAsync(int movieId, string imageUrl, bool isPrimary = false, int displayOrder = 0);
        Task<bool> DeleteMovieImageAsync(int imageId);
        Task<bool> SetPrimaryImageAsync(int movieId, int imageId);

        Task<bool> AddMovieVideoAsync(int movieId, MovieVideoCreateDto videoDto, int languageId);
        Task<bool> DeleteMovieVideoAsync(int videoId);

        Task<bool> AddSocialLinkAsync(int movieId, MovieSocialLinkCreateDto linkDto);
        Task<bool> DeleteSocialLinkAsync(int linkId);

        Task<bool> UpdateRatingAsync(int movieId, int newRating);
        Task<bool> IncrementVoteCountAsync(int movieId);

        // BULK OPERATIONS
        Task<bool> SetMovieActiveStatusAsync(int movieId, bool isActive);
        Task<bool> SetMovieFeaturedStatusAsync(int movieId, bool isFeatured);
        Task<int> GetTotalMoviesCountAsync();
    }
}