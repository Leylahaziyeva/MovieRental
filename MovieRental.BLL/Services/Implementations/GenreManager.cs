using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class GenreManager : CrudManager<Genre, GenreViewModel, GenreCreateViewModel, GenreUpdateViewModel>, IGenreService
    {
        private readonly IRepositoryAsync<Genre> _genreRepository;
        private readonly IRepositoryAsync<GenreTranslation> _translationRepository;
        private readonly IMapper _mapper;

        public GenreManager(
            IRepositoryAsync<Genre> genreRepository,
            IRepositoryAsync<GenreTranslation> translationRepository,
            IMapper mapper) : base(genreRepository, mapper)
        {
            _genreRepository = genreRepository;
            _translationRepository = translationRepository;
            _mapper = mapper;
        }

        #region Override Methods

        public override async Task<GenreViewModel> CreateAsync(GenreCreateViewModel createViewModel)
        {
            var genre = new Genre();
            var createdGenre = await _genreRepository.AddAsync(genre);

            var mainTranslation = new GenreTranslation
            {
                Name = createViewModel.Name,
                GenreId = createdGenre.Id,
                LanguageId = createViewModel.DefaultLanguageId
            };
            await _translationRepository.AddAsync(mainTranslation);

            if (createViewModel.Translations?.Any() == true)
            {
                foreach (var translation in createViewModel.Translations)
                {
                    if (translation.LanguageId == createViewModel.DefaultLanguageId)
                        continue;

                    var genreTranslation = new GenreTranslation
                    {
                        Name = translation.Name,
                        GenreId = createdGenre.Id,
                        LanguageId = translation.LanguageId
                    };
                    await _translationRepository.AddAsync(genreTranslation);
                }
            }

            var genreWithTranslations = await _genreRepository.GetAsync(
                g => g.Id == createdGenre.Id,
                include: q => q.Include(g => g.GenreTranslations)
            );

            return _mapper.Map<GenreViewModel>(genreWithTranslations);
        }

        public override async Task<bool> UpdateAsync(int id, GenreUpdateViewModel updateViewModel)
        {
            var genre = await _genreRepository.GetAsync(
                g => g.Id == id && !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations)
            );

            if (genre == null) return false;

            var defaultTranslation = genre.GenreTranslations
                .FirstOrDefault(t => t.LanguageId == updateViewModel.DefaultLanguageId && !t.IsDeleted);

            if (defaultTranslation != null)
            {
                defaultTranslation.Name = updateViewModel.Name;
                defaultTranslation.UpdatedAt = DateTime.UtcNow;
                await _translationRepository.UpdateAsync(defaultTranslation);
            }
            else
            {
                var newDefaultTranslation = new GenreTranslation
                {
                    Name = updateViewModel.Name,
                    GenreId = id,
                    LanguageId = updateViewModel.DefaultLanguageId
                };
                await _translationRepository.AddAsync(newDefaultTranslation);
            }

            if (updateViewModel.Translations?.Any() == true)
            {
                foreach (var translationVM in updateViewModel.Translations)
                {
                    if (translationVM.LanguageId == updateViewModel.DefaultLanguageId)
                        continue;

                    if (translationVM.Id > 0)
                    {
                        var existingTranslation = genre.GenreTranslations
                            .FirstOrDefault(t => t.Id == translationVM.Id && !t.IsDeleted);

                        if (existingTranslation != null)
                        {
                            existingTranslation.Name = translationVM.Name;
                            existingTranslation.UpdatedAt = DateTime.UtcNow;
                            await _translationRepository.UpdateAsync(existingTranslation);
                        }
                    }
                    else
                    {
                        var newTranslation = new GenreTranslation
                        {
                            Name = translationVM.Name,
                            GenreId = id,
                            LanguageId = translationVM.LanguageId
                        };
                        await _translationRepository.AddAsync(newTranslation);
                    }
                }
            }

            genre.UpdatedAt = DateTime.UtcNow;
            await _genreRepository.UpdateAsync(genre);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return false;

            genre.IsDeleted = true;
            genre.UpdatedAt = DateTime.UtcNow;

            await _genreRepository.UpdateAsync(genre);
            return true;
        }

        public override async Task<GenreViewModel?> GetByIdAsync(int id)
        {
            var genre = await _genreRepository.GetAsync(
                g => g.Id == id && !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations.Where(gt => !gt.IsDeleted)),
                AsNoTracking: true
            );

            if (genre == null) return null;

            return _mapper.Map<GenreViewModel>(genre);
        }

        #endregion

        #region List & Filter Methods

        public async Task<IEnumerable<GenreViewModel>> GetAllActiveAsync(int languageId)
        {
            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations.Where(gt => !gt.IsDeleted)),
                AsNoTracking: true
            );

            var filteredGenres = genres
                .Where(g => g.GenreTranslations.Any(gt => gt.LanguageId == languageId))
                .OrderBy(g => g.GenreTranslations
                    .First(gt => gt.LanguageId == languageId).Name)
                .ToList();

            var viewModels = _mapper.Map<IEnumerable<GenreViewModel>>(filteredGenres);

            foreach (var vm in viewModels)
            {
                var genre = filteredGenres.First(g => g.Id == vm.Id);
                var translation = genre.GenreTranslations
                    .FirstOrDefault(gt => gt.LanguageId == languageId);

                vm.Name = translation?.Name ?? "N/A";
            }

            return viewModels;
        }

        public async Task<GenreViewModel?> GetByIdWithLanguageAsync(int id, int languageId)
        {
            var genre = await _genreRepository.GetAsync(
                g => g.Id == id && !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations.Where(gt => !gt.IsDeleted)),
                AsNoTracking: true
            );

            if (genre == null) return null;

            var viewModel = _mapper.Map<GenreViewModel>(genre);

            var translation = genre.GenreTranslations
                .FirstOrDefault(gt => gt.LanguageId == languageId);

            viewModel.Name = translation?.Name ?? "N/A";

            return viewModel;
        }

        public async Task<IEnumerable<GenreViewModel>> SearchByNameAsync(string searchTerm, int languageId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllActiveAsync(languageId);
            }

            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations.Where(gt => !gt.IsDeleted)),
                AsNoTracking: true
            );

            var searchResults = genres
                .Where(g => g.GenreTranslations.Any(gt =>
                    gt.LanguageId == languageId &&
                    gt.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(g => g.GenreTranslations
                    .First(gt => gt.LanguageId == languageId).Name)
                .ToList();

            var viewModels = _mapper.Map<IEnumerable<GenreViewModel>>(searchResults);

            foreach (var vm in viewModels)
            {
                var genre = searchResults.First(g => g.Id == vm.Id);
                var translation = genre.GenreTranslations
                    .FirstOrDefault(gt => gt.LanguageId == languageId);

                vm.Name = translation?.Name ?? "N/A";
            }

            return viewModels;
        }

        public async Task<IEnumerable<GenreViewModel>> GetPopularGenresAsync(int languageId, int count = 10)
        {
            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                include: q => q
                    .Include(g => g.GenreTranslations.Where(gt => !gt.IsDeleted))
                    .Include(g => g.MovieGenres.Where(mg => !mg.IsDeleted)),
                AsNoTracking: true
            );

            var popularGenres = genres
                .Where(g => g.GenreTranslations.Any(gt => gt.LanguageId == languageId))
                .OrderByDescending(g => g.MovieGenres.Count)
                .Take(count)
                .ToList();

            var viewModels = _mapper.Map<IEnumerable<GenreViewModel>>(popularGenres);

            foreach (var vm in viewModels)
            {
                var genre = popularGenres.First(g => g.Id == vm.Id);
                var translation = genre.GenreTranslations
                    .FirstOrDefault(gt => gt.LanguageId == languageId);

                vm.Name = translation?.Name ?? "N/A";
            }

            return viewModels;
        }

        #endregion

        #region Translation Management

        public async Task<bool> TranslationExistsAsync(int genreId, int languageId)
        {
            var translation = await _translationRepository.GetAsync(
                gt => gt.GenreId == genreId &&
                      gt.LanguageId == languageId &&
                      !gt.IsDeleted
            );

            return translation != null;
        }

        public async Task<bool> AddTranslationAsync(int genreId, GenreTranslationCreateViewModel translation)
        {
            var exists = await TranslationExistsAsync(genreId, translation.LanguageId);
            if (exists) return false;

            var genreTranslation = new GenreTranslation
            {
                Name = translation.Name,
                GenreId = genreId,
                LanguageId = translation.LanguageId
            };

            await _translationRepository.AddAsync(genreTranslation);
            return true;
        }

        public async Task<bool> UpdateTranslationAsync(int translationId, GenreTranslationUpdateViewModel translation)
        {
            var genreTranslation = await _translationRepository.GetAsync(
                gt => gt.Id == translationId && !gt.IsDeleted
            );

            if (genreTranslation == null) return false;

            genreTranslation.Name = translation.Name;
            genreTranslation.UpdatedAt = DateTime.UtcNow;

            await _translationRepository.UpdateAsync(genreTranslation);
            return true;
        }

        public async Task<bool> DeleteTranslationAsync(int translationId)
        {
            var translation = await _translationRepository.GetByIdAsync(translationId);
            if (translation == null) return false;

            translation.IsDeleted = true;
            translation.UpdatedAt = DateTime.UtcNow;

            await _translationRepository.UpdateAsync(translation);
            return true;
        }

        #endregion

        #region Stats & Utility Methods

        public async Task<int> GetGenreMovieCountAsync(int genreId)
        {
            var genre = await _genreRepository.GetAsync(
                g => g.Id == genreId && !g.IsDeleted,
                include: q => q.Include(g => g.MovieGenres.Where(mg => !mg.IsDeleted)),
                AsNoTracking: true
            );

            return genre?.MovieGenres.Count ?? 0;
        }

        public async Task<int> GetTotalGenresCountAsync()
        {
            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                AsNoTracking: true
            );

            return genres.Count();
        }

        #endregion
    }
}