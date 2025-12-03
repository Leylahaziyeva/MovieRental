using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

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

        public override async Task<GenreViewModel> CreateAsync(GenreCreateViewModel createViewModel)
        {
            var genre = new Genre();
            var createdGenre = await _genreRepository.AddAsync(genre);

            var mainTranslation = new GenreTranslation
            {
                Name = createViewModel.Name,
                GenreId = createdGenre.Id,
                LanguageId = 1
            };
            await _translationRepository.AddAsync(mainTranslation);

            if (createViewModel.Translations != null && createViewModel.Translations.Any())
            {
                foreach (var translation in createViewModel.Translations)
                {
                    var genreTranslation = new GenreTranslation
                    {
                        Name = translation.Name,
                        GenreId = createdGenre.Id,
                        LanguageId = translation.LanguageId
                    };
                    await _translationRepository.AddAsync(genreTranslation);
                }
            }

            return _mapper.Map<GenreViewModel>(createdGenre);
        }

        public override async Task<bool> UpdateAsync(int id, GenreUpdateViewModel updateViewModel)
        {
            var genre = await _genreRepository.GetAsync(
                g => g.Id == id && !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations)
            );

            if (genre == null) return false;


            if (updateViewModel.Translations != null)
            {
                foreach (var translationVM in updateViewModel.Translations)
                {
                    var existingTranslation = genre.GenreTranslations
                        .FirstOrDefault(t => t.LanguageId == translationVM.LanguageId);

                    if (existingTranslation != null)
                    {
                        existingTranslation.Name = translationVM.Name;
                        await _translationRepository.UpdateAsync(existingTranslation);
                    }
                    else
                    {
                        var newTranslation = new GenreTranslation
                        {
                            Name = translationVM.Name,
                            GenreId = genre.Id,
                            LanguageId = translationVM.LanguageId
                        };
                        await _translationRepository.AddAsync(newTranslation);
                    }
                }
            }

            await _genreRepository.UpdateAsync(genre);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return false;

            genre.IsDeleted = true;
            genre.UpdatedAt = DateTime.Now;

            await _genreRepository.UpdateAsync(genre);
            return true;
        }

        public async Task<IEnumerable<GenreViewModel>> GetAllActiveAsync(int languageId)
        {
            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                include: q => q.Include(g => g.GenreTranslations.Where(gt => gt.LanguageId == languageId)),
                orderBy: q => q.OrderBy(g => g.GenreTranslations.First().Name)
            );

            return _mapper.Map<IEnumerable<GenreViewModel>>(genres);
        }

        public async Task<IEnumerable<GenreViewModel>> SearchByNameAsync(string searchTerm, int languageId)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllActiveAsync(languageId);
            }

            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted &&
                    g.GenreTranslations.Any(gt =>
                        gt.LanguageId == languageId &&
                        gt.Name.Contains(searchTerm)),
                include: q => q.Include(g => g.GenreTranslations.Where(gt => gt.LanguageId == languageId)),
                orderBy: q => q.OrderBy(g => g.GenreTranslations.First().Name)
            );

            return _mapper.Map<IEnumerable<GenreViewModel>>(genres);
        }

        public async Task<IEnumerable<GenreViewModel>> GetPopularGenresAsync(int languageId, int count = 10)
        {
            var genres = await _genreRepository.GetAllAsync(
                predicate: g => !g.IsDeleted,
                include: q => q
                    .Include(g => g.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                    .Include(g => g.MovieGenres),
                orderBy: q => q.OrderByDescending(g => g.MovieGenres.Count)
            );

            return _mapper.Map<IEnumerable<GenreViewModel>>(genres.Take(count));
        }

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
            genreTranslation.UpdatedAt = DateTime.Now;

            await _translationRepository.UpdateAsync(genreTranslation);
            return true;
        }

        public async Task<bool> DeleteTranslationAsync(int translationId)
        {
            var translation = await _translationRepository.GetByIdAsync(translationId);
            if (translation == null) return false;

            translation.IsDeleted = true;
            translation.UpdatedAt = DateTime.Now;

            await _translationRepository.UpdateAsync(translation);
            return true;
        }
    }
}