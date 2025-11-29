using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class MovieManager : CrudManager<Movie, MovieViewModel, MovieCreateViewModel, MovieUpdateViewModel>, IMovieService
    {
        private readonly IRepositoryAsync<Movie> _movieRepository;
        private readonly IRepositoryAsync<MovieTranslation> _translationRepository;
        private readonly IRepositoryAsync<MovieGenre> _movieGenreRepository;
        private readonly IRepositoryAsync<MoviePerson> _moviePersonRepository;
        private readonly IRepositoryAsync<MovieImage> _movieImageRepository;
        private readonly IRepositoryAsync<MovieVideo> _movieVideoRepository;
        private readonly IRepositoryAsync<MovieVideoTranslation> _movieVideoTranslationRepository;
        private readonly IRepositoryAsync<MovieSocialLink> _socialLinkRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public MovieManager(
            IRepositoryAsync<Movie> movieRepository,
            IRepositoryAsync<MovieTranslation> translationRepository,
            IRepositoryAsync<MovieGenre> movieGenreRepository,
            IRepositoryAsync<MoviePerson> moviePersonRepository,
            IRepositoryAsync<MovieImage> movieImageRepository,
            IRepositoryAsync<MovieVideo> movieVideoRepository,
            IRepositoryAsync<MovieVideoTranslation> movieVideoTranslationRepository,
            IRepositoryAsync<MovieSocialLink> socialLinkRepository,
            ICloudinaryService cloudinaryService,
            IMapper mapper) : base(movieRepository, mapper)
        {
            _movieRepository = movieRepository;
            _translationRepository = translationRepository;
            _movieGenreRepository = movieGenreRepository;
            _moviePersonRepository = moviePersonRepository;
            _movieImageRepository = movieImageRepository;
            _movieVideoRepository = movieVideoRepository;
            _movieVideoTranslationRepository = movieVideoTranslationRepository;
            _socialLinkRepository = socialLinkRepository;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        public override async Task<MovieViewModel> CreateAsync(MovieCreateViewModel createViewModel)
        {
            var posterUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.PosterImage);
            var coverUrl = await _cloudinaryService.ImageCreateAsync(createViewModel.CoverImage);
            var videoUrl = await _cloudinaryService.VideoUploadAsync(createViewModel.Video);

            var movie = _mapper.Map<Movie>(createViewModel);
            movie.PosterImageUrl = posterUrl;
            movie.CoverImageUrl = coverUrl;
            movie.VideoUrl = videoUrl;

            var createdMovie = await _movieRepository.AddAsync(movie);

            var translation = new MovieTranslation
            {
                Title = createViewModel.Title,
                Plot = createViewModel.Plot,
                Director = createViewModel.DirectorNames,   
                Writers = createViewModel.WriterNames,      
                Cast = createViewModel.CastNames,           
                MovieId = createdMovie.Id,
                LanguageId = createViewModel.LanguageId
            };
            await _translationRepository.AddAsync(translation);

            if (createViewModel.SelectedGenreIds != null)
            {
                foreach (var genreId in createViewModel.SelectedGenreIds)
                {
                    await AddGenreToMovieAsync(createdMovie.Id, genreId);
                }
            }

            if (createViewModel.SelectedActorIds != null)
            {
                foreach (var actorId in createViewModel.SelectedActorIds)
                {
                    await AddActorToMovieAsync(createdMovie.Id, actorId);
                }
            }

            if (createViewModel.SelectedDirectorIds != null)
            {
                foreach (var directorId in createViewModel.SelectedDirectorIds)
                {
                    await AddDirectorToMovieAsync(createdMovie.Id, directorId);
                }
            }

            if (createViewModel.SelectedWriterIds != null)
            {
                foreach (var writerId in createViewModel.SelectedWriterIds)
                {
                    await AddWriterToMovieAsync(createdMovie.Id, writerId);
                }
            }

            if (createViewModel.AdditionalImages != null && createViewModel.AdditionalImages.Any())
            {
                var imageUrls = await _cloudinaryService.ImageCreateAsync(createViewModel.AdditionalImages);
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    await AddMovieImageAsync(createdMovie.Id, imageUrls[i], false, i + 1);
                }
            }

            if (createViewModel.SocialLinks != null && createViewModel.SocialLinks.Any())
            {
                foreach (var link in createViewModel.SocialLinks)
                {
                    await AddSocialLinkAsync(createdMovie.Id, link);
                }
            }

            return _mapper.Map<MovieViewModel>(createdMovie);
        }

        public override async Task<bool> UpdateAsync(int id, MovieUpdateViewModel updateViewModel)
        {
            var movie = await _movieRepository.GetAsync(
                m => m.Id == id,
                include: q => q.Include(m => m.MovieTranslations)
            );

            if (movie == null) return false;

            if (updateViewModel.PosterImage != null)
            {
                await _cloudinaryService.ImageDeleteAsync(movie.PosterImageUrl);
                movie.PosterImageUrl = await _cloudinaryService.ImageCreateAsync(updateViewModel.PosterImage);
            }

            if (updateViewModel.CoverImage != null)
            {
                await _cloudinaryService.ImageDeleteAsync(movie.CoverImageUrl);
                movie.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(updateViewModel.CoverImage);
            }

            if (updateViewModel.Video != null)
            {
                await _cloudinaryService.VideoDeleteAsync(movie.VideoUrl);
                movie.VideoUrl = await _cloudinaryService.VideoUploadAsync(updateViewModel.Video);
            }

            movie.Year = updateViewModel.Year;
            movie.Duration = updateViewModel.Duration;
            movie.ReleaseDate = updateViewModel.ReleaseDate;
            movie.Budget = updateViewModel.Budget;
            movie.LovePercentage = updateViewModel.LovePercentage;
            movie.VotesCount = updateViewModel.VotesCount;
            movie.RentalPrice = updateViewModel.RentalPrice;
            movie.RentalDurationDays = updateViewModel.RentalDurationDays;
            movie.IsAvailableForRent = updateViewModel.IsAvailableForRent;
            movie.Format = updateViewModel.Format;
            movie.IsActive = updateViewModel.IsActive;
            movie.IsFeatured = updateViewModel.IsFeatured;
            movie.TrailerUrl = updateViewModel.TrailerUrl;
            movie.LanguageId = updateViewModel.LanguageId;
            movie.CurrencyId = updateViewModel.CurrencyId;

            var translation = movie.MovieTranslations.FirstOrDefault(t => t.LanguageId == updateViewModel.LanguageId);
            if (translation != null)
            {
                translation.Title = updateViewModel.Title ?? translation.Title;
                translation.Plot = updateViewModel.Plot ?? translation.Plot;
                translation.Director = updateViewModel.DirectorNames ?? translation.Director;   
                translation.Writers = updateViewModel.WriterNames ?? translation.Writers;       
                translation.Cast = updateViewModel.CastNames ?? translation.Cast;               
                await _translationRepository.UpdateAsync(translation);
            }

            if (updateViewModel.SelectedGenreIds != null)
            {
                var existingGenres = await _movieGenreRepository.GetAllAsync(mg => mg.MovieId == id);
                foreach (var genre in existingGenres)
                {
                    await _movieGenreRepository.DeleteAsync(genre);
                }
                foreach (var genreId in updateViewModel.SelectedGenreIds)
                {
                    await AddGenreToMovieAsync(id, genreId);
                }
            }

            if (updateViewModel.SelectedActorIds != null ||
                updateViewModel.SelectedDirectorIds != null ||
                updateViewModel.SelectedWriterIds != null)
            {
                var existingPersons = await _moviePersonRepository.GetAllAsync(mp => mp.MovieId == id);
                foreach (var person in existingPersons)
                {
                    await _moviePersonRepository.DeleteAsync(person);
                }

                if (updateViewModel.SelectedActorIds != null)
                {
                    foreach (var actorId in updateViewModel.SelectedActorIds)
                    {
                        await AddActorToMovieAsync(id, actorId);
                    }
                }

                if (updateViewModel.SelectedDirectorIds != null)
                {
                    foreach (var directorId in updateViewModel.SelectedDirectorIds)
                    {
                        await AddDirectorToMovieAsync(id, directorId);
                    }
                }

                if (updateViewModel.SelectedWriterIds != null)
                {
                    foreach (var writerId in updateViewModel.SelectedWriterIds)
                    {
                        await AddWriterToMovieAsync(id, writerId);
                    }
                }
            }

            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        public async Task<MovieDetailsViewModel?> GetMovieDetailsAsync(int movieId, int languageId)
        {
            var movie = await _movieRepository.GetAsync(
                m => m.Id == movieId && m.IsActive,
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == languageId))
                    .Include(m => m.Currency)
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(m => m.MoviePersons).ThenInclude(mp => mp.Person).ThenInclude(p => p!.PersonTranslations!.Where(pt => pt.LanguageId == languageId))
                    .Include(m => m.MovieImages.Where(mi => mi.IsActive))
                    .Include(m => m.MovieVideos.Where(mv => mv.IsActive)).ThenInclude(mv => mv.MovieVideoTranslations.Where(mvt => mvt.LanguageId == languageId))
                    .Include(m => m.MovieSocialLinks.Where(sl => sl.IsActive))
            );

            if (movie == null) return null;

            return _mapper.Map<MovieDetailsViewModel>(movie);
        }

        public async Task<MoviesListViewModel> GetFilteredMoviesAsync(MovieFilterViewModel filter)
        {
            var query = await _movieRepository.GetAllAsync(
                predicate: m => m.IsActive,
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == filter.CurrentLanguageId))
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == filter.CurrentLanguageId))
                    .Include(m => m.Currency)
            );

            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                query = query.Where(m => m.MovieTranslations.Any(t => t.Title.Contains(filter.SearchQuery))).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Genre))
            {
                query = query.Where(m => m.MovieGenres.Any(mg => mg.Genre!.GenreTranslations.Any(gt => gt.Name == filter.Genre))).ToList();
            }

            if (filter.Year.HasValue)
            {
                query = query.Where(m => m.Year == filter.Year.Value).ToList();
            }

            if (!string.IsNullOrEmpty(filter.Format))
            {
                query = query.Where(m => m.Format == filter.Format).ToList();
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(m => m.RentalPrice <= filter.MaxPrice.Value).ToList();
            }

            query = filter.Sort switch
            {
                "price_asc" => query.OrderBy(m => m.RentalPrice).ToList(),
                "price_desc" => query.OrderByDescending(m => m.RentalPrice).ToList(),
                "rating" => query.OrderByDescending(m => m.LovePercentage).ToList(),
                "latest" => query.OrderByDescending(m => m.ReleaseDate).ToList(),
                _ => query.OrderByDescending(m => m.Id).ToList()
            };

            var totalCount = query.Count();
            var movies = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new MoviesListViewModel
            {
                Movies = _mapper.Map<IEnumerable<MovieViewModel>>(movies),
                Filter = filter,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<MovieViewModel>> GetFeaturedMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetAllAsync(
                predicate: m => m.IsActive && m.IsFeatured,
                orderBy: q => q.OrderByDescending(m => m.LovePercentage),
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == languageId))
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(m => m.Currency)
            );

            return _mapper.Map<IEnumerable<MovieViewModel>>(movies.Take(count));
        }

        public async Task<IEnumerable<MovieViewModel>> GetLatestMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetAllAsync(
                predicate: m => m.IsActive && m.ReleaseDate <= DateTime.Now,
                orderBy: q => q.OrderByDescending(m => m.ReleaseDate),
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == languageId))
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(m => m.Currency)
            );

            return _mapper.Map<IEnumerable<MovieViewModel>>(movies.Take(count));
        }

        public async Task<IEnumerable<MovieViewModel>> GetPopularMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetAllAsync(
                predicate: m => m.IsActive,
                orderBy: q => q.OrderByDescending(m => m.VotesCount).ThenByDescending(m => m.LovePercentage),
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == languageId))
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(m => m.Currency)
            );

            return _mapper.Map<IEnumerable<MovieViewModel>>(movies.Take(count));
        }

        public async Task<IEnumerable<MovieViewModel>> GetUpcomingMoviesAsync(int languageId, int count = 10)
        {
            var movies = await _movieRepository.GetAllAsync(
                predicate: m => m.IsActive && m.ReleaseDate > DateTime.Now,
                orderBy: q => q.OrderBy(m => m.ReleaseDate),
                include: q => q
                    .Include(m => m.MovieTranslations.Where(t => t.LanguageId == languageId))
                    .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ThenInclude(g => g!.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(m => m.Currency)
            );

            return _mapper.Map<IEnumerable<MovieViewModel>>(movies.Take(count));
        }

        public async Task<bool> AddActorToMovieAsync(int movieId, int personId, string? characterName = null, int displayOrder = 0)
        {
            var exists = await _moviePersonRepository.GetAsync(mp =>
                mp.MovieId == movieId &&
                mp.PersonId == personId &&
                mp.Role == MoviePersonRole.Actor);

            if (exists != null) return false;

            var moviePerson = new MoviePerson
            {
                MovieId = movieId,
                PersonId = personId,
                Role = MoviePersonRole.Actor,
                CharacterName = characterName,
                DisplayOrder = displayOrder,
                IsActive = true
            };

            await _moviePersonRepository.AddAsync(moviePerson);
            return true;
        }

        public async Task<bool> AddDirectorToMovieAsync(int movieId, int personId)
        {
            var exists = await _moviePersonRepository.GetAsync(mp =>
                mp.MovieId == movieId &&
                mp.PersonId == personId &&
                mp.Role == MoviePersonRole.Director);

            if (exists != null) return false;

            var moviePerson = new MoviePerson
            {
                MovieId = movieId,
                PersonId = personId,
                Role = MoviePersonRole.Director,
                DisplayOrder = 0,
                IsActive = true
            };

            await _moviePersonRepository.AddAsync(moviePerson);
            return true;
        }

        public async Task<bool> AddWriterToMovieAsync(int movieId, int personId)
        {
            var exists = await _moviePersonRepository.GetAsync(mp =>
                mp.MovieId == movieId &&
                mp.PersonId == personId &&
                mp.Role == MoviePersonRole.Writer);

            if (exists != null) return false;

            var moviePerson = new MoviePerson
            {
                MovieId = movieId,
                PersonId = personId,
                Role = MoviePersonRole.Writer,
                DisplayOrder = 0,
                IsActive = true
            };

            await _moviePersonRepository.AddAsync(moviePerson);
            return true;
        }

        public async Task<bool> RemovePersonFromMovieAsync(int movieId, int personId, MoviePersonRole role)
        {
            var moviePerson = await _moviePersonRepository.GetAsync(mp =>
                mp.MovieId == movieId &&
                mp.PersonId == personId &&
                mp.Role == role);

            if (moviePerson == null) return false;

            await _moviePersonRepository.DeleteAsync(moviePerson);
            return true;
        }

        public async Task<bool> AddGenreToMovieAsync(int movieId, int genreId)
        {
            var exists = await _movieGenreRepository.GetAsync(mg =>
                mg.MovieId == movieId && mg.GenreId == genreId);

            if (exists != null) return false;

            var movieGenre = new MovieGenre
            {
                MovieId = movieId,
                GenreId = genreId
            };

            await _movieGenreRepository.AddAsync(movieGenre);
            return true;
        }

        public async Task<bool> RemoveGenreFromMovieAsync(int movieId, int genreId)
        {
            var movieGenre = await _movieGenreRepository.GetAsync(mg =>
                mg.MovieId == movieId && mg.GenreId == genreId);

            if (movieGenre == null) return false;

            await _movieGenreRepository.DeleteAsync(movieGenre);
            return true;
        }

        public async Task<List<int>> GetMovieGenreIdsAsync(int movieId)
        {
            var movieGenres = await _movieGenreRepository.GetAllAsync(mg => mg.MovieId == movieId);
            return movieGenres.Select(mg => mg.GenreId).ToList();
        }


        public async Task<bool> AddMovieImageAsync(int movieId, string imageUrl, bool isPrimary = false, int displayOrder = 0)
        {
            if (isPrimary)
            {
                var existingPrimary = await _movieImageRepository.GetAllAsync(mi =>
                    mi.MovieId == movieId && mi.IsPrimary);

                foreach (var img in existingPrimary)
                {
                    img.IsPrimary = false;
                    await _movieImageRepository.UpdateAsync(img);
                }
            }

            var movieImage = new MovieImage
            {
                MovieId = movieId,
                ImageUrl = imageUrl,
                IsPrimary = isPrimary,
                DisplayOrder = displayOrder,
                IsActive = true
            };

            await _movieImageRepository.AddAsync(movieImage);
            return true;
        }

        public async Task<bool> DeleteMovieImageAsync(int imageId)
        {
            var image = await _movieImageRepository.GetByIdAsync(imageId);
            if (image == null) return false;

            await _cloudinaryService.ImageDeleteAsync(image.ImageUrl);
            await _movieImageRepository.DeleteAsync(image);
            return true;
        }

        public async Task<bool> SetPrimaryImageAsync(int movieId, int imageId)
        {
            var existingPrimary = await _movieImageRepository.GetAllAsync(mi =>
                mi.MovieId == movieId && mi.IsPrimary);

            foreach (var img in existingPrimary)
            {
                img.IsPrimary = false;
                await _movieImageRepository.UpdateAsync(img);
            }

            var newPrimary = await _movieImageRepository.GetByIdAsync(imageId);
            if (newPrimary == null || newPrimary.MovieId != movieId) return false;

            newPrimary.IsPrimary = true;
            await _movieImageRepository.UpdateAsync(newPrimary);
            return true;
        }

        public async Task<bool> AddMovieVideoAsync(int movieId, MovieVideoCreateDto videoDto, int languageId)
        {
            var videoUrl = await _cloudinaryService.VideoUploadAsync(videoDto.VideoFile);

            var movieVideo = new MovieVideo
            {
                MovieId = movieId,
                VideoUrl = videoUrl,
                VideoType = (VideoType)videoDto.VideoType,
                DisplayOrder = 0,
                IsActive = true
            };

            var createdVideo = await _movieVideoRepository.AddAsync(movieVideo);

            var translation = new MovieVideoTranslation
            {
                Title = videoDto.Title,
                Description = videoDto.Description,
                MovieVideoId = createdVideo.Id,
                LanguageId = languageId
            };

            await _movieVideoTranslationRepository.AddAsync(translation);
            return true;
        }

        public async Task<bool> DeleteMovieVideoAsync(int videoId)
        {
            var video = await _movieVideoRepository.GetByIdAsync(videoId);
            if (video == null) return false;

            await _cloudinaryService.VideoDeleteAsync(video.VideoUrl);
            await _movieVideoRepository.DeleteAsync(video);
            return true;
        }

        public async Task<bool> AddSocialLinkAsync(int movieId, MovieSocialLinkCreateDto linkDto)
        {
            var socialLink = _mapper.Map<MovieSocialLink>(linkDto);
            socialLink.MovieId = movieId;
            socialLink.IsActive = true;

            await _socialLinkRepository.AddAsync(socialLink);
            return true;
        }

        public async Task<bool> DeleteSocialLinkAsync(int linkId)
        {
            var link = await _socialLinkRepository.GetByIdAsync(linkId);
            if (link == null) return false;

            await _socialLinkRepository.DeleteAsync(link);
            return true;
        }


        public async Task<bool> UpdateRatingAsync(int movieId, int newRating)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null) return false;

            movie.LovePercentage = newRating;
            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        public async Task<bool> IncrementVoteCountAsync(int movieId)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null) return false;

            movie.VotesCount++;
            await _movieRepository.UpdateAsync(movie);
            return true;
        }


        public async Task<bool> SetMovieActiveStatusAsync(int movieId, bool isActive)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null) return false;

            movie.IsActive = isActive;
            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        public async Task<bool> SetMovieFeaturedStatusAsync(int movieId, bool isFeatured)
        {
            var movie = await _movieRepository.GetByIdAsync(movieId);
            if (movie == null) return false;

            movie.IsFeatured = isFeatured;
            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        public async Task<int> GetTotalMoviesCountAsync()
        {
            var movies = await _movieRepository.GetAllAsync(m => m.IsActive);
            return movies.Count();
        }
    }
}