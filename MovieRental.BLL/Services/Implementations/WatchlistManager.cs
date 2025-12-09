using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.Watchlist;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class WatchlistManager : IWatchlistService
    {
        private readonly IRepositoryAsync<UserWatchlist> _watchlistRepository;
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly ICookieService _cookieService;

        public WatchlistManager(
            IRepositoryAsync<UserWatchlist> watchlistRepository,
            IRepositoryAsync<Movie> movieRepository,
            ICookieService cookieService)
        {
            _watchlistRepository = watchlistRepository;
            _movieRepository = movieRepository;
            _cookieService = cookieService;
        }

        public async Task<WatchlistViewModel> GetUserWatchlistAsync(string userId, int page = 1, int pageSize = 12)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var (items, totalCount) = await _watchlistRepository.GetPagedAsync(
                predicate: w => w.UserId == userId,
                orderBy: q => q.OrderByDescending(w => w.AddedDate),
                include: q => q
                    .Include(w => w.Movie)
                        .ThenInclude(m => m!.MovieGenres)
                            .ThenInclude(mg => mg.Genre)
                                .ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(w => w.Movie)
                        .ThenInclude(m => m!.MovieTranslations.Where(mt => mt.LanguageId == languageId))
                    .Include(w => w.Movie)
                        .ThenInclude(m => m!.Language),
                page: page,
                pageSize: pageSize,
                AsNoTracking: true
            );

            var movies = items
                .Where(w => w.Movie != null)
                .Select(w => MapToMovieViewModel(w.Movie!, languageId))
                .ToList();

            return new WatchlistViewModel
            {
                Movies = movies,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<bool> AddToWatchlistAsync(string userId, int movieId)
        {
            var exists = await _watchlistRepository.GetAsync(
                w => w.UserId == userId && w.MovieId == movieId
            );

            if (exists != null)
                return false;

            var watchlistItem = new UserWatchlist
            {
                UserId = userId,
                MovieId = movieId,
                AddedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _watchlistRepository.AddAsync(watchlistItem);
            return true;
        }

        public async Task<bool> RemoveFromWatchlistAsync(string userId, int movieId)
        {
            var item = await _watchlistRepository.GetAsync(
                w => w.UserId == userId && w.MovieId == movieId
            );

            if (item == null)
                return false;

            await _watchlistRepository.DeleteAsync(item);
            return true;
        }

        public async Task<bool> IsInWatchlistAsync(string userId, int movieId)
        {
            var item = await _watchlistRepository.GetAsync(
                w => w.UserId == userId && w.MovieId == movieId
            );

            return item != null;
        }

        private MovieViewModel MapToMovieViewModel(Movie movie, int languageId)
        {
            var movieTranslation = movie.MovieTranslations?
                .FirstOrDefault(mt => mt.LanguageId == languageId);

            var genres = movie.MovieGenres?
                .Where(mg => mg.Genre != null && !mg.Genre.IsDeleted)
                .Select(mg => {
                    var genreTranslation = mg.Genre!.GenreTranslations?
                        .FirstOrDefault(gt => gt.LanguageId == languageId);

                    return new GenreViewModel
                    {
                        Id = mg.Genre.Id,
                        Name = genreTranslation?.Name ?? "Unknown",
                        GenreTranslations = new List<GenreTranslationViewModel>()
                    };
                })
                .ToList() ?? new List<GenreViewModel>();

            return new MovieViewModel
            {
                Id = movie.Id,
                Title = movieTranslation?.Title ?? "Unknown Title",
                PosterImageUrl = movie.PosterImageUrl ?? "/img/default-poster.jpg",
                //CoverImageUrl = movie.CoverImageUrl ?? "/img/default-cover.jpg",
                Year = movie.Year,
                Duration = movie.Duration,
                RentalPrice = movie.RentalPrice,
                CurrencyCode = movie.Currency?.IsoCode ?? "USD",
                CurrencySymbol = movie.Currency?.Symbol ?? "$",
                FormattedPrice = $"{movie.Currency?.Symbol ?? "$"}{movie.RentalPrice:F2}",
                LovePercentage = movie.LovePercentage,
                VotesCount = movie.VotesCount,
                Format = movie.Format ?? "2D",
                Genres = genres,
                IsFeatured = movie.IsFeatured,
                IsAvailableForRent = movie.IsAvailableForRent,
                ReleaseDate = movie.ReleaseDate,
                LanguageName = movie.Language?.LanguageTranslations
                    .FirstOrDefault(lt => lt.LanguageId == languageId)?.Name ?? "Unknown"
            };
        }
    }
}