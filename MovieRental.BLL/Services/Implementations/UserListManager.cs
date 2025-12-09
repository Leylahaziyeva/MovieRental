using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.UserList;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class UserListManager : IUserListService
    {
        private readonly IRepositoryAsync<UserList> _userListRepository;
        private readonly IRepositoryAsync<UserListMovie> _userListMovieRepository;
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly ICookieService _cookieService;

        public UserListManager(
            IRepositoryAsync<UserList> userListRepository,
            IRepositoryAsync<UserListMovie> userListMovieRepository,
            IRepositoryAsync<Movie> movieRepository,
            ICookieService cookieService)
        {
            _userListRepository = userListRepository;
            _userListMovieRepository = userListMovieRepository;
            _movieRepository = movieRepository;
            _cookieService = cookieService;
        }

        public async Task<UserListsViewModel> GetUserListsAsync(string userId, int page = 1, int pageSize = 12)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var (lists, totalCount) = await _userListRepository.GetPagedAsync(
                predicate: ul => ul.UserId == userId,
                orderBy: q => q.OrderByDescending(ul => ul.CreatedAt),
                include: q => q.Include(ul => ul.UserListMovies)!,
                page: page,
                pageSize: pageSize,
                AsNoTracking: true
            );

            var userLists = new List<UserListViewModel>();

            foreach (var list in lists)
            {
                var movieCount = list.UserListMovies?.Count ?? 0;

                var movieIds = list.UserListMovies?
                    .OrderByDescending(ulm => ulm.AddedDate)
                    .Take(4)
                    .Select(ulm => ulm.MovieId)
                    .ToList() ?? new List<int>();

                var movies = new List<MovieViewModel>();

                if (movieIds.Any())
                {
                    var movieEntities = await _movieRepository.GetAllAsync(
                        predicate: m => movieIds.Contains(m.Id),
                        include: q => q
                            .Include(m => m.MovieGenres)
                                .ThenInclude(mg => mg.Genre)
                                    .ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                            .Include(m => m.MovieTranslations.Where(mt => mt.LanguageId == languageId))
                            .Include(m => m.Language),
                        AsNoTracking: true
                    );

                    movies = movieEntities.Select(m => MapToMovieViewModel(m, languageId)).ToList();
                }

                userLists.Add(new UserListViewModel
                {
                    Id = list.Id,
                    Name = list.Name,
                    Description = list.Description,
                    IsPublic = list.IsPublic,
                    CreatedAt = list.CreatedAt,
                    MovieCount = movieCount,
                    Movies = movies
                });
            }

            return new UserListsViewModel
            {
                Lists = userLists,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<UserListViewModel?> GetUserListByIdAsync(int listId, string userId)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var list = await _userListRepository.GetAsync(
                predicate: ul => ul.Id == listId && ul.UserId == userId,
                include: q => q.Include(ul => ul.UserListMovies)!,
                AsNoTracking: true
            );

            if (list == null)
                return null;

            var movieIds = list.UserListMovies?.Select(ulm => ulm.MovieId).ToList() ?? new List<int>();

            var movies = new List<MovieViewModel>();

            if (movieIds.Any())
            {
                var movieEntities = await _movieRepository.GetAllAsync(
                    predicate: m => movieIds.Contains(m.Id),
                    include: q => q
                        .Include(m => m.MovieGenres)
                            .ThenInclude(mg => mg.Genre)
                                .ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                        .Include(m => m.MovieTranslations.Where(mt => mt.LanguageId == languageId))
                        .Include(m => m.Language),
                    AsNoTracking: true
                );

                movies = movieEntities.Select(m => MapToMovieViewModel(m, languageId)).ToList();
            }

            return new UserListViewModel
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                IsPublic = list.IsPublic,
                CreatedAt = list.CreatedAt,
                MovieCount = list.UserListMovies?.Count ?? 0,
                Movies = movies
            };
        }

        public async Task<int> CreateUserListAsync(string userId, CreateUserListViewModel model)
        {
            var userList = new UserList
            {
                UserId = userId,
                Name = model.Name,
                Description = model.Description,
                IsPublic = model.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userListRepository.AddAsync(userList);
            return result.Id;
        }

        public async Task<bool> UpdateUserListAsync(int listId, string userId, CreateUserListViewModel model)
        {
            var list = await _userListRepository.GetAsync(
                ul => ul.Id == listId && ul.UserId == userId
            );

            if (list == null)
                return false;

            list.Name = model.Name;
            list.Description = model.Description;
            list.IsPublic = model.IsPublic;
            list.UpdatedAt = DateTime.UtcNow;

            await _userListRepository.UpdateAsync(list);
            return true;
        }

        public async Task<bool> DeleteUserListAsync(int listId, string userId)
        {
            var list = await _userListRepository.GetAsync(
                ul => ul.Id == listId && ul.UserId == userId
            );

            if (list == null)
                return false;

            await _userListRepository.DeleteAsync(list);
            return true;
        }

        public async Task<bool> AddMovieToListAsync(int listId, int movieId, string userId)
        {
            var list = await _userListRepository.GetAsync(
                ul => ul.Id == listId && ul.UserId == userId
            );

            if (list == null)
                return false;

            var exists = await _userListMovieRepository.GetAsync(
                ulm => ulm.UserListId == listId && ulm.MovieId == movieId
            );

            if (exists != null)
                return false;

            var userListMovie = new UserListMovie
            {
                UserListId = listId,
                MovieId = movieId,
                AddedDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _userListMovieRepository.AddAsync(userListMovie);
            return true;
        }

        public async Task<bool> RemoveMovieFromListAsync(int listId, int movieId, string userId)
        {
            var list = await _userListRepository.GetAsync(
                ul => ul.Id == listId && ul.UserId == userId
            );

            if (list == null)
                return false;

            var item = await _userListMovieRepository.GetAsync(
                ulm => ulm.UserListId == listId && ulm.MovieId == movieId
            );

            if (item == null)
                return false;

            await _userListMovieRepository.DeleteAsync(item);
            return true;
        }

        public async Task<bool> IsMovieInListAsync(int listId, int movieId, string userId)
        {
            var list = await _userListRepository.GetAsync(
                ul => ul.Id == listId && ul.UserId == userId
            );

            if (list == null)
                return false;

            var item = await _userListMovieRepository.GetAsync(
                ulm => ulm.UserListId == listId && ulm.MovieId == movieId
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