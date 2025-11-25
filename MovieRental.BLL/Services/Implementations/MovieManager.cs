using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Actor;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.MovieImage;
using MovieRental.BLL.ViewModels.MovieSocialLink;
using MovieRental.BLL.ViewModels.MovieVideo;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class MovieManager : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieManager(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task<MoviesListViewModel> GetFilteredMoviesAsync(MovieFilterViewModel filter)
        {
            var predicate = BuildFilterPredicate(filter);

            var include = BuildIncludeExpression(filter.CurrentLanguageId);

            var orderBy = BuildOrderByExpression(filter.Sort);

            var movies = await _movieRepository.GetFilteredMoviesAsync(
                predicate,
                include,
                orderBy,
                take: 50);

            var movieCards = movies.Select(m => MapToMovieCard(m, filter.CurrentLanguageId)).ToList();

            var pageTitle = BuildPageTitle(filter);

            return new MoviesListViewModel
            {
                Movies = movieCards,
                Filter = filter,
                TotalCount = movieCards.Count,
                PageTitle = pageTitle
            };
        }

        public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int movieId, int languageId)
        {
            var movie = await _movieRepository.GetMovieWithDetailsAsync(movieId, languageId);

            if (movie == null) return null;

            return MapToMovieDetails(movie, languageId);
        }

        public async Task<IEnumerable<MovieCardViewModel>> GetFeaturedMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetFilteredMoviesAsync(
                predicate: m => !m.IsDeleted && m.IsActive && m.IsFeatured,
                include: BuildIncludeExpression(languageId),
                orderBy: q => q.OrderByDescending(m => m.CreatedAt),
                take: count);

            return movies.Select(m => MapToMovieCard(m, languageId));
        }

        public async Task<IEnumerable<MovieCardViewModel>> GetLatestMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetFilteredMoviesAsync(
                predicate: m => !m.IsDeleted && m.IsActive,
                include: BuildIncludeExpression(languageId),
                orderBy: q => q.OrderByDescending(m => m.ReleaseDate),
                take: count);

            return movies.Select(m => MapToMovieCard(m, languageId));
        }

        public async Task<IEnumerable<MovieCardViewModel>> GetPopularMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetFilteredMoviesAsync(
                predicate: m => !m.IsDeleted && m.IsActive,
                include: BuildIncludeExpression(languageId),
                orderBy: q => q.OrderByDescending(m => m.LovePercentage)
                              .ThenByDescending(m => m.VotesCount),
                take: count);

            return movies.Select(m => MapToMovieCard(m, languageId));
        }


        // PRIVATE HELPER METHODS

        private Expression<Func<Movie, bool>> BuildFilterPredicate(MovieFilterViewModel filter)
        {
            Expression<Func<Movie, bool>> predicate = m => !m.IsDeleted && m.IsActive;

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                var searchLower = filter.SearchQuery.ToLower();
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) &&
                    m.MovieTranslations.Any(mt =>
                        mt.Title.ToLower().Contains(searchLower) ||
                        (mt.Plot != null && mt.Plot.ToLower().Contains(searchLower))
                    );
            }

            if (!string.IsNullOrEmpty(filter.Genre))
            {
                var genreName = filter.Genre.ToLower();
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) &&
                    m.MovieGenres.Any(mg => mg.Genre.GenreTranslations
                        .Any(gt => gt.Name.ToLower() == genreName));
            }

            if (!string.IsNullOrEmpty(filter.Language))
            {
                var languageName = filter.Language.ToLower();
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) &&
                    m.Language.Name.ToLower() == languageName;
            }

            if (filter.Year.HasValue)
            {
                var year = filter.Year.Value;
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) && m.Year == year;
            }

            if (!string.IsNullOrEmpty(filter.Format))
            {
                var format = filter.Format;
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) && m.Format == format;
            }

            if (filter.MaxPrice.HasValue)
            {
                var maxPrice = filter.MaxPrice.Value;
                var oldPredicate = predicate;
                predicate = m => oldPredicate.Compile()(m) && m.RentalPrice <= maxPrice;
            }

            return predicate;
        }

        private Func<IQueryable<Movie>, IIncludableQueryable<Movie, object>> BuildIncludeExpression(int languageId)
        {
            return query => query
                .Include(m => m.MovieTranslations.Where(mt => mt.LanguageId == languageId))
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .ThenInclude(g => g.GenreTranslations.Where(gt => gt.LanguageId == languageId));
        }

        private Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? BuildOrderByExpression(string? sort)
        {
            return sort?.ToLower() switch
            {
                "popular" => q => q.OrderByDescending(m => m.LovePercentage)
                                   .ThenByDescending(m => m.VotesCount),
                "rating" => q => q.OrderByDescending(m => m.LovePercentage),
                "latest" => q => q.OrderByDescending(m => m.ReleaseDate),
                "price-low" => q => q.OrderBy(m => m.RentalPrice),
                "price-high" => q => q.OrderByDescending(m => m.RentalPrice),
                "year" => q => q.OrderByDescending(m => m.Year),
                _ => q => q.OrderByDescending(m => m.CreatedAt)
            };
        }

        private MovieCardViewModel MapToMovieCard(Movie movie, int languageId)
        {
            return new MovieCardViewModel
            {
                Id = movie.Id,
                Title = movie.MovieTranslations.FirstOrDefault()?.Title ?? "Untitled",
                PosterImageUrl = movie.PosterImageUrl,
                Year = movie.Year,
                Duration = movie.Duration,
                RentalPrice = movie.RentalPrice,
                LovePercentage = movie.LovePercentage,
                VotesCount = movie.VotesCount,
                Format = movie.Format ?? "2D",
                IsFeatured = movie.IsFeatured,
                IsAvailableForRent = movie.IsAvailableForRent,
                Genres = movie.MovieGenres
                    .Select(mg => mg.Genre.GenreTranslations.FirstOrDefault()?.Name ?? "Genre")
                    .ToList()
            };
        }

        private MovieDetailsViewModel MapToMovieDetails(Movie movie, int languageId)
        {
            var translation = movie.MovieTranslations.FirstOrDefault();

            return new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = translation?.Title ?? "Untitled",
                Plot = translation?.Plot ?? "",
                Director = translation?.Director ?? "",
                Writers = translation?.Writers ?? "",
                Cast = translation?.Cast ?? "",
                PosterImageUrl = movie.PosterImageUrl,
                CoverImageUrl = movie.CoverImageUrl,
                VideoUrl = movie.VideoUrl,
                TrailerUrl = movie.TrailerUrl,
                Year = movie.Year,
                Duration = movie.Duration,
                ReleaseDate = movie.ReleaseDate,
                Budget = movie.Budget,
                LovePercentage = movie.LovePercentage,
                VotesCount = movie.VotesCount,
                RentalPrice = movie.RentalPrice,
                RentalDurationDays = movie.RentalDurationDays,
                IsAvailableForRent = movie.IsAvailableForRent,
                Format = movie.Format,
                Genres = movie.MovieGenres.Select(mg => new GenreViewModel
                {
                    Id = mg.Genre.Id,
                    Name = mg.Genre.GenreTranslations.FirstOrDefault()?.Name ?? "Genre",
                    IconClass = mg.Genre.IconClass
                }).ToList(),
                Actors = movie.MovieActors
                    .OrderBy(ma => ma.DisplayOrder)
                    .Select(ma => new ActorViewModel
                    {
                        Id = ma.Actor.Id,
                        FullName = ma.Actor.ActorTranslations.FirstOrDefault()?.FullName ?? "Unknown",
                        ProfileImageUrl = ma.Actor.ProfileImageUrl,
                        Role = ma.Role,
                        Category = ma.Category,
                        DisplayOrder = ma.DisplayOrder
                    }).ToList(),
                Images = movie.MovieImages
                    .OrderBy(mi => mi.DisplayOrder)
                    .Select(mi => new MovieImageViewModel
                    {
                        ImageUrl = mi.ImageUrl,
                        IsPrimary = mi.IsPrimary,
                        DisplayOrder = mi.DisplayOrder
                    }).ToList(),
                Videos = movie.MovieVideos
                    .Where(mv => mv.IsActive)
                    .OrderBy(mv => mv.DisplayOrder)
                    .Select(mv => new MovieVideoViewModel
                    {
                        VideoUrl = mv.VideoUrl,
                        Title = mv.MovieVideoTranslations.FirstOrDefault()?.Title ?? "",
                        Description = mv.MovieVideoTranslations.FirstOrDefault()?.Description,
                        VideoType = mv.VideoType,
                        ThumbnailUrl = mv.ThumbnailUrl
                    }).ToList(),
                SocialLinks = movie.MovieSocialLinks
                    .Where(sl => sl.IsActive)
                    .OrderBy(sl => sl.DisplayOrder)
                    .Select(sl => new MovieSocialLinkViewModel
                    {
                        Platform = sl.Platform,
                        Url = sl.Url,
                        IconClass = sl.IconClass
                    }).ToList()
            };
        }

        private string BuildPageTitle(MovieFilterViewModel filter)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(filter.Language))
                parts.Add(filter.Language);

            if (!string.IsNullOrEmpty(filter.Genre))
                parts.Add(filter.Genre);

            if (filter.Year.HasValue)
                parts.Add(filter.Year.Value.ToString());

            parts.Add("Movies");

            return string.Join(" ", parts);
        }

        public async Task<IEnumerable<MovieCardViewModel>> GetUpcomingMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetFilteredMoviesAsync(
                predicate: m => !m.IsDeleted && m.IsActive && m.ReleaseDate >= DateTime.Now,
                include: BuildIncludeExpression(languageId),
                orderBy: q => q.OrderBy(m => m.ReleaseDate),
                take: count);

            return movies.Select(m => MapToMovieCard(m, languageId));
        }
    }
}